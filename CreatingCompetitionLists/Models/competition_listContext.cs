using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CreatingCompetitionLists.Models
{
    public partial class competition_listContext : DbContext
    {
        public competition_listContext()
        {
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Database=competition_list;Username=postgres;Password=123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Direction>(entity =>
            {
                entity.ToTable("direction");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CountForEnrollee).HasColumnName("count_for_enrollee");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.ShortTitle)
                    .HasColumnType("character varying")
                    .HasColumnName("short_title");

                entity.Property(e => e.Title).HasColumnType("character varying");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Directions)
                    .HasForeignKey(d => d.FacultyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("direction_faculty_id_fk");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("faculty");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Title).HasColumnType("character varying");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
