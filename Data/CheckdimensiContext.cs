using System;
using System.Collections.Generic;
using AplikasiCheckDimensi.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace AplikasiCheckDimensi.Data;

public partial class CheckdimensiContext : DbContext
{
    public CheckdimensiContext()
    {
    }

    public CheckdimensiContext(DbContextOptions<CheckdimensiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InputAktual> InputAktuals { get; set; }

    public virtual DbSet<Produk> Produks { get; set; }

    public virtual DbSet<StandarDimensi> StandarDimensis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InputAktual>(entity =>
        {
            entity.ToTable("InputAktual");

            entity.HasIndex(e => e.StandarDimensiId, "IX_InputAktual_StandarDimensiId");

            entity.HasOne(d => d.StandarDimensi).WithMany(p => p.InputAktuals).HasForeignKey(d => d.StandarDimensiId);
        });

        modelBuilder.Entity<Produk>(entity =>
        {
            entity.ToTable("Produk");
        });

        modelBuilder.Entity<StandarDimensi>(entity =>
        {
            entity.ToTable("StandarDimensi");

            entity.HasIndex(e => e.ProdukId, "IX_StandarDimensi_ProdukId");

            entity.Property(e => e.DimensiAMax).HasColumnName("DimensiA_Max");
            entity.Property(e => e.DimensiAMin).HasColumnName("DimensiA_Min");
            entity.Property(e => e.DimensiBMax).HasColumnName("DimensiB_Max");
            entity.Property(e => e.DimensiBMin).HasColumnName("DimensiB_Min");
            entity.Property(e => e.InnerDiameterMax).HasColumnName("InnerDiameter_Max");
            entity.Property(e => e.InnerDiameterMin).HasColumnName("InnerDiameter_Min");
            entity.Property(e => e.OuterDiameterMax).HasColumnName("OuterDiameter_Max");
            entity.Property(e => e.OuterDiameterMin).HasColumnName("OuterDiameter_Min");
            entity.Property(e => e.PanjangMax).HasColumnName("Panjang_Max");
            entity.Property(e => e.PanjangMin).HasColumnName("Panjang_Min");
            entity.Property(e => e.RadiusMax).HasColumnName("Radius_Max");
            entity.Property(e => e.RadiusMin).HasColumnName("Radius_Min");
            entity.Property(e => e.SudutMax).HasColumnName("Sudut_Max");
            entity.Property(e => e.SudutMin).HasColumnName("Sudut_Min");
            entity.Property(e => e.ThicknessMax).HasColumnName("Thickness_Max");
            entity.Property(e => e.ThicknessMin).HasColumnName("Thickness_Min");
            entity.Property(e => e.TinggiMax).HasColumnName("Tinggi_Max");
            entity.Property(e => e.TinggiMin).HasColumnName("Tinggi_Min");

            entity.HasOne(d => d.Produk).WithMany(p => p.StandarDimensis).HasForeignKey(d => d.ProdukId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
