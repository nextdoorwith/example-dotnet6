# 既存のテーブル定義からDBContext, Entityを生成

# dotnet ef dbcontext scaffoldヘルプ
#   https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet

<#
  --data-annotations: 
    テーブル名・カラム名等をEntityのアノテーションとして付与
  --no-pluralize:
    DBContextのDbSetプロパティ名(テーブル名)を複数形の名前にしない
    (Entityと名前が異なると混乱するので)
  --no-onconfiguring:
    OnConfiguring()メソッドの出力を抑制する。
    partial定義した独自クラス側のOnConfiguring()を使用するため。
#>

cd ..

$constr = "Server=localhost;Initial Catalog=DbExample;Integrated Security=true;TrustServerCertificate=true"
dotnet ef dbcontext scaffold "${constr}" `
	Microsoft.EntityFrameworkCore.SqlServer `
	--context AppDbContext --context-dir Context `
	--output-dir Entity `
	--data-annotations --no-pluralize --no-onconfiguring --force

pause
