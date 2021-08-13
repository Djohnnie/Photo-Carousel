using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.Entities;
using PhotoCarousel.Entities.Enums;

namespace PhotoCarousel.DataAccess
{
    public class PhotoCarouselDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<Photo> Photos { get; set; }

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
                a.HasKey(x => x.Id).IsClustered(false);
                a.HasIndex(x => x.SysId).IsUnique().IsClustered();
                a.Property(x => x.SysId).ValueGeneratedOnAdd();
                a.Property(x => x.Orientation).HasConversion(new EnumToStringConverter<Orientation>());
            });
        }
    }
}