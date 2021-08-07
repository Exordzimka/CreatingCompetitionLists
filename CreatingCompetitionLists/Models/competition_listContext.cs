using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace CreatingCompetitionLists.Models
{
    public partial class competition_listContext : DbContext
    {
        private ConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();
        private IConfiguration _configuration;

        public competition_listContext()
        {
            _configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            _configurationBuilder.AddJsonFile("appsettings.json");
            _configuration = _configurationBuilder.Build();
        }

        public competition_listContext(DbContextOptions<competition_listContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                var connectionSection = _configuration.GetSection("MySqlConnection");
                var connectionString = connectionSection.GetChildren().Aggregate("",
                    (current, attribute) => current + $"{attribute.Key}={attribute.Value};");
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Direction>(entity =>
            {
                entity.ToTable("direction");

                entity.HasIndex(e => e.FacultyId, "direction_faculty_id_fk");

                entity.HasIndex(e => e.Id, "direction_pk")
                    .IsUnique();

                entity.Property(e => e.CountForEnrollee)
                    .HasColumnType("int(11)")
                    .HasColumnName("count_for_enrollee");

                entity.Property(e => e.FacultyId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("faculty_id");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.ShortTitle)
                    .HasMaxLength(50)
                    .HasColumnName("short_title");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Title");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Directions)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("direction_faculty_id_fk");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("faculty");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}