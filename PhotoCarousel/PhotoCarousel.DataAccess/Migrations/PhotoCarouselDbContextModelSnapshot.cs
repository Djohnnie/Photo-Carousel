﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhotoCarousel.DataAccess;

namespace PhotoCarousel.DataAccess.Migrations
{
    [DbContext(typeof(PhotoCarouselDbContext))]
    partial class PhotoCarouselDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PhotoCarousel.Entities.History", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PhotoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Scheduled")
                        .HasColumnType("datetime2");

                    b.Property<int>("SysId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .IsClustered(false);

                    b.HasIndex("Scheduled");

                    b.HasIndex("SysId")
                        .IsUnique()
                        .IsClustered();

                    b.ToTable("HISTORY");
                });

            modelBuilder.Entity("PhotoCarousel.Entities.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTaken")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Orientation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rating")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Sha256Hash")
                        .HasMaxLength(32)
                        .HasColumnType("varbinary(32)");

                    b.Property<string>("SourcePath")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SysId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ThumbnailPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .IsClustered(false);

                    b.HasIndex("Sha256Hash");

                    b.HasIndex("SourcePath");

                    b.HasIndex("SysId")
                        .IsUnique()
                        .IsClustered();

                    b.HasIndex("Sha256Hash", "SourcePath");

                    b.ToTable("PHOTOS");
                });
#pragma warning restore 612, 618
        }
    }
}
