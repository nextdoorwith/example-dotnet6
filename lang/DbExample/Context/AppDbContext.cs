using System;
using System.Collections.Generic;
using DbExample.Entity;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MEmployee> MEmployee { get; set; }

    public virtual DbSet<MOrder> MOrder { get; set; }

    public virtual DbSet<MOrderDetail> MOrderDetail { get; set; }

    public virtual DbSet<MProduct> MProduct { get; set; }

    public virtual DbSet<TSales> TSales { get; set; }

    public virtual DbSet<ZTestNopk> ZTestNopk { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MEmployee>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("pk_m_employee");

            entity.Property(e => e.Version)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<MOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("pk_m_order");

            entity.Property(e => e.Version)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<MOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("pk_m_order_detail");

            entity.Property(e => e.Version)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Order).WithMany(p => p.MOrderDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_m_order_detail_order_id");

            entity.HasOne(d => d.Product).WithMany(p => p.MOrderDetail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_m_order_detail_product_id");
        });

        modelBuilder.Entity<MProduct>(entity =>
        {
            entity.HasKey(e => new { e.Type, e.Id }).HasName("pk_m_product");

            entity.Property(e => e.Version)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TSales>(entity =>
        {
            entity.HasKey(e => new { e.RegionId, e.Year, e.Month }).HasName("pk_t_sales");

            entity.Property(e => e.Expense).HasDefaultValueSql("((0))");
            entity.Property(e => e.Profit).HasDefaultValueSql("((0))");
            entity.Property(e => e.Revenue).HasDefaultValueSql("((0))");
            entity.Property(e => e.Version)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
