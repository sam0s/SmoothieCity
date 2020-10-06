using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SmoothieDemo.Models
{
    public partial class SmoothieCityContext : DbContext
    {
        public SmoothieCityContext()
        {
        }

        public SmoothieCityContext(DbContextOptions<SmoothieCityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Smoothies> Smoothies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.ItemNumber)
                    .HasName("PK__Cart__C28ACDB6BFACCBDC");

                entity.Property(e => e.ItemNumber).ValueGeneratedNever();

                entity.Property(e => e.Sid).HasColumnName("SID");

                entity.HasOne(d => d.S)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.Sid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cart_Smoothies");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CustomerName).HasColumnType("text");
            });

            modelBuilder.Entity<Smoothies>(entity =>
            {
                entity.HasKey(e => e.SmoothieId)
                    .HasName("PK__Smoothie__E37525566D194F8D");

                entity.Property(e => e.SmoothieId)
                    .HasColumnName("SmoothieID")
                    .ValueGeneratedNever();

                entity.Property(e => e.SmoothieImage).HasColumnType("text");

                entity.Property(e => e.SmoothieName).HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
