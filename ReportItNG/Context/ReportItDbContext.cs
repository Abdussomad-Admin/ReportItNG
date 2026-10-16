using Microsoft.EntityFrameworkCore;
using ReportIt.Api.Models;

namespace ReportItNG.Context
{
    public class ReportItDbContext(DbContextOptions<ReportItDbContext> options) : DbContext(options)
    {
        public DbSet<Report> Reports => Set<Report>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(report => report.Id);

                entity.HasIndex(report => report.Reference)
                    .IsUnique();

                entity.Property(report => report.Reference)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(report => report.Title)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(report => report.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(report => report.Location)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(report => report.ImagePath)
                    .HasMaxLength(500);

                entity.Property(report => report.Category)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(report => report.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(report => report.CreatedAtUtc)
                    .IsRequired();
            });
        }
    }
}