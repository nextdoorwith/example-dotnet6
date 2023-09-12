using DbExample.common;
using DbExample.Context;
using DbExample.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;

using Console = DbExample.common.TestHelper.Console; // System.Consoleの模倣

namespace DbExample
{
    [Collection(nameof(DbExample))]
    public class EfCoreExampleTests : IClassFixture<DbExampleTestFixture>
    {
        public EfCoreExampleTests(ITestOutputHelper output)
            => TestHelper.SetOutput(output);

        [Fact]
        public void ConnectTest()
        {
            using var context = new AppDbContext();
            context.Database.ExecuteSqlRaw("select 1");
        }

        [Fact]
        public async void CrudTest()
        {
            using var context = new AppDbContext();
            await context.Database.EnsureCreatedAsync();

            var e1 = new MEmployee()
            {
                EmployeeNo = "A12345",
                Name = "山田太郎",
                Birthday = new DateTime(2000, 4, 1)

            };
            context.Add(e1);
            await context.SaveChangesAsync();

            var registedList = await context.MEmployee.ToArrayAsync();
            Console.WriteLine($"retrieved: {registedList.Length}");
            foreach (var e in registedList)
                Console.WriteLine($"user_id={e.UserId}, name={e.Name}, birthday={e.Birthday}");

            e1.Birthday = e1.Birthday.AddYears(1);
            await context.SaveChangesAsync();

            var updatedList = await context.MEmployee.ToArrayAsync();
            Console.WriteLine($"retrieved: {updatedList.Length}");
            foreach (var e in updatedList)
                Console.WriteLine($"user_id={e.UserId}, name={e.Name}, birthday={e.Birthday}");

            context.Remove(e1);
            await context.SaveChangesAsync();

            var deletedList = await context.MEmployee.ToArrayAsync();
            Console.WriteLine($"retrieved: {deletedList.Length}");
        }

        [Fact]
        public void DumpTableColumnsTest()
        {
            using var context = new AppDbContext();

            var type = typeof(MEmployee);
            var entityType = context.Model.FindEntityType(type); // GetEntityTypes()で一覧取得も可

            // テーブル名
            var tableName = entityType.GetTableName();

            // カラムを出力
            var columns = entityType.GetProperties();
            foreach (var prop in columns)
            {
                // GetColumnName()は、EFCore6ではObsolete(非推奨)で、次のように取得
                // var columnName = c.GetColumnName(StoreObjectIdentifier.Table(tableName));
                // ※ EFCore7では非推奨ではない
                var colName = prop.GetColumnName();
                var colType = prop.GetColumnType();
                var nullable = prop.IsNullable ? "NULL" : "NOT NULL";

                Console.WriteLine($"{tableName}.{colName}: {colType}, {nullable}");
            }
        }

        [Fact]
        public void DumpPrimaryKeysTest()
        {
            using var context = new AppDbContext();
            var entityType = context.Model.FindEntityType(typeof(TSales));

            // エンティティからの主キー情報の取得
            // ※PKがないテーブルの場合、FindPrimaryKey()はnull
            var pkProps = entityType.FindPrimaryKey()?.Properties;
            foreach (var p in pkProps)
            {
                var colName = p.GetColumnName();
                var colType = p.GetColumnType();
                Console.WriteLine($"{p.Name}[{p.ClrType}]: {colName}[{colType}]");
            }

            Console.WriteLine("---");

            // 各プロパティを主キーか判定
            var props = entityType.GetProperties();
            foreach(var p in props)
            {
                Console.WriteLine($"{p.Name}[{p.ClrType}]: {p.IsPrimaryKey()}");
            }
        }

        [Fact]
        public void DumpForeignKeysTest()
        {
            using var context = new AppDbContext();
            var entityType = context.Model.FindEntityType(typeof(MOrderDetail));

            // エンティティからの外部キー情報の取得
            var fksFromEntity = entityType.GetForeignKeys();
            foreach (var fk in fksFromEntity) DumpForeignKey(fk);

            Console.WriteLine("---");

            // プロパティを指定した外部キー情報の取得
            var prop = entityType.FindProperty(nameof(MOrderDetail.OrderId));
            var fksFromProp = entityType.FindForeignKeys(prop);
            foreach (var fk in fksFromProp) DumpForeignKey(fk);

            Console.WriteLine("---");

            // 各プロパティからの外部キー情報の取得
            var props = entityType.GetProperties();
            foreach (var p in props)
            {
                var fks = p.GetContainingForeignKeys();
                foreach (var fk in fks) DumpForeignKey(fk);
            }

        }
        // 外部キーはIForeignKeyで表現される
        private void DumpForeignKey(IForeignKey fk)
        {
            // IReadOnlyForeignKey.cs(ToDebugString())を参考
            var fromKeys = fk.Properties.Select(e => e.GetColumnName());
            var toTable = fk.PrincipalEntityType.GetTableName();
            var toKeys = fk.PrincipalKey.Properties.Select(e => e.GetColumnName());
            //
            var fromKeysStr = string.Join(", ", fromKeys);
            var toKeysStr = string.Join(", ", toKeys);
            Console.WriteLine($"[{fromKeysStr}] -> {toTable}[{toKeysStr}]");
        }

        [Fact]
        public async Task TruncateTablesTest()
        {
            // 外部キー参照先テーブルはtruncateできないのでdeleteする。
            // https://learn.microsoft.com/ja-jp/sql/t-sql/statements/truncate-table-transact-sql?view=sql-server-ver16#restrictions
            // (参考) trunateするとMsg 4712エラーになる。
            //     "FOREIGN KEY 制約でテーブル 'tablename' が参照されているので、このテーブルは切り捨てられません。"
            //      "Cannot truncate table 'tablename' because it is being referenced by a FOREIGN KEY constraint."

            using var context = new AppDbContext();
            var entities = context.Model.GetEntityTypes();

            // 外部キー参照先テーブル一覧(truncate不可テーブル)
            var fkToTables = entities
                .SelectMany(e => e.GetForeignKeys().Select(k => k.PrincipalEntityType.GetTableName()))
                .Where(e => !string.IsNullOrEmpty(e))
                .Distinct();
            // 外部キーなしテーブル一覧(truncate可テーブル)
            var noFkTables = entities
                .Select(e => e.GetTableName())
                .Where(e => !string.IsNullOrEmpty(e))
                .Except(fkToTables);

            var truncates = noFkTables.Select(e => $"truncate table [{e}]");
            var deletes = fkToTables.Select(e => $"delete from [{e}]");
            foreach (var sql in truncates.Concat(deletes))
            {
                Console.WriteLine(sql);
                await context.Database.ExecuteSqlRawAsync(sql);
            }
        }

        [Fact]
        public async Task UpdateNullColumnsTest()
        {
            using var context = new AppDbContext();
            var e = new TSales()
            {
                RegionId = 1,
                Year = 2023,
                Revenue = null, // default: 0
                Expense = 500, // default: 0
                Profit = -500 // default: 0
            };
            await context.AddAsync(e);

            // 既定値指定がありnullが設定されているプロパティをnullにするSQLを生成する。
            // （SaveChangesAsync()すると、Entityに既定値が反映されてしまうので
            // 反映前にnull更新用SQLを生成）
            var (nullUpdateSql, nullUpdatePs) = CreateNullUpdateSql(context, e);

            await context.SaveChangesAsync();

            // 既定値あり項目をnullで更新する。
            var affected = await context.Database.ExecuteSqlRawAsync(nullUpdateSql, nullUpdatePs);
            Assert.Equal(1, affected);

            // AsNoTracking()またはcontext.ChangeTracker.Clear()しないと、
            // 登録データ(Entity)がキャッシュされ、最新のDB値を取得できない
            //context.ChangeTracker.Clear();
            var r1 = await context.TSales.AsNoTracking().SingleAsync();
            Console.WriteLine($"before: {r1.Revenue}, {r1.Expense}, {r1.Profit}");

            var r2 = await context.TSales.AsNoTracking().SingleAsync();
            Console.WriteLine($"after : {r2.Revenue}, {r2.Expense}, {r2.Profit}");
        }

        private (string, object[]) CreateNullUpdateSql(DbContext context, object entity)
        {
            var modelType = context.Model.FindEntityType(entity.GetType());
            var modelProps = modelType.GetProperties();
            var piDic = modelType.ClrType.GetProperties().ToDictionary(k => k.Name, v => v);

            // null更新対象カラムの特定
            var nullCols = modelProps
                .Where(e => !string.IsNullOrEmpty(e.GetDefaultValueSql())) // 既定値がある列
                .Where(e => piDic[e.Name].GetValue(entity) == null) // 値がnullのプロパティ
                .Select(e => e.GetColumnName());
            if (!nullCols.Any()) return (null, null);

            // キーカラム・値の抽出
            // (PKは存在する前提、存在しない場合はFindPrimaryKey()がnull)
            var keyProps = modelType.FindPrimaryKey()?.Properties;
            var keyCols = keyProps.Select(p => p.GetColumnName());
            var keyVals = keyProps.Select(p => piDic[p.Name].GetValue(entity));

            // SQL文とパラメータの構築
            // ex) "update XXX set c1={0}, c2={0}, ... where k1={1} and k2={2}, ..."
            var table = modelType.GetTableName();
            var setCols = nullCols.Select(c => $"{c} = {{0}}");
            var whrCols = keyCols.Select((k, i) => $"{k}={{{i + 1}}}");
            var sql =
                $"update {table} " +
                $"set {string.Join(", ", setCols)} " +
                $"where {string.Join(" AND ", whrCols)}";
            var prms = new object[] { null }.Concat(keyVals).ToArray();

            Console.WriteLine($"sql=\"{sql}\"");
            Console.WriteLine($"params={{{string.Join(",", prms)}}}");
            return (sql, prms);
        }

        [Fact]
        public async void CreateInsertableTest()
        {
            using var context = new AppDbContext();
            var e = CreateInsertableEntity<MEmployee>(context);
            var p = CreateInsertableEntity<MProduct>(context);
            await context.AddRangeAsync(e, p);
            await context.SaveChangesAsync();
        }
        public T CreateInsertableEntity<T>(DbContext context) where T : class
        {
            // 本来はクラス変数等で別定義
            var excludes = "createdby,createdon,updatedby,updatedon,version"
                .Split(",").ToHashSet(StringComparer.OrdinalIgnoreCase);

            var type = typeof(T);
            var target = (T)Activator.CreateInstance(type);

            var modelType = context.Model.FindEntityType(type);
            var vgs = new[] { ValueGenerated.OnAdd, ValueGenerated.OnAddOrUpdate };
            foreach (var mp in modelType.GetProperties())
            {
                // null許容、ValueGenerated(Identity, rowversion等の値指定不可列)、
                // 監査用情報を格納する業務独自列等は対象外
                var propName = mp.Name;
                if (mp.IsNullable ||
                    vgs.Contains(mp.ValueGenerated) || excludes.Contains(propName)) continue;

                // NotNull列は、その型に応じて適当な値を設定
                var propInfo = type.GetProperty(propName);
                var propType = propInfo.PropertyType;
                object value;
                if (propType == typeof(string)) value = "str";
                else if (propType == typeof(byte)) value = (byte)1;
                else if (propType == typeof(short)) value = (short)2;
                else if (propType == typeof(int)) value = 3;
                else if (propType == typeof(long)) value = 4L;
                else if (propType == typeof(float)) value = 5f;
                else if (propType == typeof(double)) value = 6d;
                else if (propType == typeof(decimal)) value = 7m;
                else if (propType == typeof(DateTime)) value = new DateTime(2000, 1, 1);
                else if (propType == typeof(bool)) value = true;
                else throw new NotImplementedException($"unknown type: {propType.FullName}");
                Console.WriteLine($"{propName}({propType.Name}) = {value}");
                propInfo.SetValue(target, value);
            }
            return target;
        }

        [Fact]
        public async Task DynamicDbSetTest()
        {
            using var context = new AppDbContext();

            await DumpCountAsync<MEmployee>(context);
            await DumpCountAsync<MOrder>(context);
        }
        private async Task DumpCountAsync<T>(AppDbContext context) where T : class
        {
            var count = await context.Set<T>().CountAsync();
            Console.WriteLine($"{typeof(T).Name}: {count}");
        }

    }

}

