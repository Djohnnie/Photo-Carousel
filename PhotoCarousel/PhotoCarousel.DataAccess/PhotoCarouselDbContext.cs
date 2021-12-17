using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.Entities;
using PhotoCarousel.Enums;

namespace PhotoCarousel.DataAccess
{
    public class PhotoCarouselDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Photo> Photos { get; set; }
        public DbSet<History> History { get; set; }

        public PhotoCarouselDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>(a =>
            {
                a.ToTable("PHOTOS");
                a.HasKey(p => p.Id).IsClustered(false);
                a.HasIndex(p => p.SysId).IsUnique().IsClustered();
                a.HasIndex(p => p.Sha256Hash);
                a.HasIndex(p => p.SourcePath);
                a.HasIndex(p => new { p.Sha256Hash, p.SourcePath });
                a.HasIndex(p => p.FolderPath);
                a.Property(p => p.SysId).ValueGeneratedOnAdd();
                a.Property(p => p.Orientation).HasConversion(new EnumToStringConverter<Orientation>());
                a.Property(p => p.Rating).HasConversion(new EnumToStringConverter<Rating>());
                a.Property(p => p.Sha256Hash).HasMaxLength(32);
                a.Property(p => p.DateIndexed).HasDefaultValue(DateTime.UtcNow);
            });

            modelBuilder.Entity<History>(a =>
            {
                a.ToTable("HISTORY");
                a.HasKey(p => p.Id).IsClustered(false);
                a.HasIndex(p => p.SysId).IsUnique().IsClustered();
                a.HasIndex(p => p.Scheduled);
            });
        }
    }
}