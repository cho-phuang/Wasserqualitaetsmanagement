using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Projekt_Schuler
{
    public partial class wasserContext : DbContext
    {
        public wasserContext()
        {
        }

        public wasserContext(DbContextOptions<wasserContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pool> Pools { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<WaterQualityDatum> WaterQualityData { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-JJAKV1E;Initial Catalog=wasser;Integrated Security=SSPI");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pool>(entity =>
            {
                entity.Property(e => e.PoolId).HasColumnName("PoolID");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.PoolName).HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Pools)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_pools_user");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4B04CA290")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<WaterQualityDatum>(entity =>
            {
                entity.HasKey(e => e.DataId)
                    .HasName("PK__WaterQua__9D05305D300E4E94");

                entity.Property(e => e.DataId).HasColumnName("DataID");

                entity.Property(e => e.ChlorineLevel).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.EntryDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Phvalue)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("PHValue");

                entity.Property(e => e.PoolId).HasColumnName("PoolID");

                entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.Turbidity).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WaterQualityData)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_quality_user");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
