using DbExample.Entity;
using DbExample.Entity.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbExample.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
        SavingChanges += AppDbContext_SavingChanges;
    }

    private const string ConnStr =
        @"Server=localhost;Initial Catalog=DbExample;Integrated Security=true;TrustServerCertificate=true";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnStr);


        // https://learn.microsoft.com/ja-jp/ef/core/logging-events-diagnostics/interceptors

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MEmployee>()
            .Property(p => p.Version)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate(); // timestamp型は自動設定(既定値指定不可)なので更新から除外
    }

    private void AppDbContext_SavingChanges([AllowNull] object sender, SavingChangesEventArgs e)
    {
        var now = DateTime.Now;
        var userid = "test";
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            // 追跡対象のエンティティへのアクセス - EF Core | Microsoft Learn
            // https://learn.microsoft.com/ja-jp/ef/core/change-tracking/entity-entries
#if false
            if (entry.Entity is not IEntity auditable) continue;
            if (entry.State == EntityState.Added)
            {
                auditable.CreatedOn = auditable.UpdatedOn = now;
                auditable.CreatedBy = auditable.UpdatedBy = userid;
            }
            else if (entry.State == EntityState.Modified)
            {
                auditable.UpdatedOn = now;
                auditable.UpdatedBy = userid;
            }
#else
            if ( entry.State == EntityState.Added )
            {
                var propVals = entry.CurrentValues;
                propVals["CreatedOn"] = propVals["UpdatedOn"] = now;
                propVals["CreatedBy"] = propVals["UpdatedBy"] = userid;
            }
            else if( entry.State == EntityState.Modified)
            {
                var propVals = entry.CurrentValues;
                propVals["UpdatedOn"] = now;
                propVals["UpdatedBy"] = userid;
            }
#endif
        }
    }

}
