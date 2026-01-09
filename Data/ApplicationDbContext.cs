using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produk> Produk { get; set; }
        public DbSet<StandarDimensi> StandarDimensi { get; set; }
        public DbSet<InputAktual> InputAktual { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<QCHoseData> QCHoseData { get; set; }
        public DbSet<MasterData> MasterData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapping untuk User - sesuai dengan struktur database SQLServer_Setup.sql
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("UserId");
                entity.Property(e => e.TanggalDibuat).HasColumnName("CreatedAt");
            });

            // Mapping untuk Produk - sesuai dengan struktur database SQLServer_Setup.sql
            modelBuilder.Entity<Produk>(entity =>
            {
                entity.ToTable("Produk");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ProdukId");
                entity.Property(e => e.Operator).HasColumnName("OperatorPIC").IsRequired(false);
                entity.Property(e => e.TanggalInput).HasColumnName("CreatedAt");
                entity.Property(e => e.NamaProduk).IsRequired();
                // Mapping kolom yang ada di SQL tapi belum di model bisa di-ignore atau di-map manual
            });

            // Mapping untuk StandarDimensi - sesuai dengan struktur database SQLServer_Setup.sql
            modelBuilder.Entity<StandarDimensi>(entity =>
            {
                entity.ToTable("StandarDimensi");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("StandarDimensiId");
            });

            // Mapping untuk InputAktual - Id sudah sama
            modelBuilder.Entity<InputAktual>(entity =>
            {
                entity.ToTable("InputAktual");
                entity.HasKey(e => e.Id);
                // StandarDimensiId sudah sama
            });

            // Konfigurasi relasi
            modelBuilder.Entity<StandarDimensi>()
                .HasOne(s => s.Produk)
                .WithMany()
                .HasForeignKey(s => s.ProdukId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InputAktual>()
                .HasOne(i => i.StandarDimensi)
                .WithMany(s => s.InputAktualls)
                .HasForeignKey(i => i.StandarDimensiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mapping untuk QCHoseData
            modelBuilder.Entity<QCHoseData>(entity =>
            {
                entity.ToTable("QCHoseData");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("QCHoseDataId");
                entity.Property(e => e.TanggalInput).HasColumnName("CreatedAt");
            });

            // Mapping untuk MasterData
            modelBuilder.Entity<MasterData>(entity =>
            {
                entity.ToTable("MasterData");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("MasterDataId");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.HasIndex(e => new { e.Tipe, e.Nilai }).IsUnique();
            });
        }
    }
}
