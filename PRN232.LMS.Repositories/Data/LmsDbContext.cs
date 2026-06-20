using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public partial class LmsDbContext : DbContext
{
    public LmsDbContext()
    {
    }

    public LmsDbContext(DbContextOptions<LmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PK_User");

            entity.ToTable("User");

            // Username: varchar(50), unique
            entity.HasIndex(e => e.Username, "UQ_User_Username")
                .IsUnique();

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);          // varchar

            // PasswordHash: varchar(255)
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            // Role: varchar(20) — vd: "Admin", "Student"
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId)
                .HasName("PK_RefreshToken");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime");

            entity.Property(e => e.IsRevoked)
                .HasDefaultValue(false);

            // Mỗi token nên unique để tra cứu refresh
            entity.HasIndex(e => e.Token, "UQ_RefreshToken_Token")
                .IsUnique();

            // FK: RefreshToken -> User
            entity.HasOne(d => d.User)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)   // xóa user thì xóa token
                .HasConstraintName("FK_RefreshToken_User");
        });
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A78496457E");

            entity.ToTable("Course");

            entity.Property(e => e.CourseName).HasMaxLength(100);

            entity.HasOne(d => d.Semester).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Semester");

            entity.HasOne(d => d.Subject).WithMany(p => p.Courses)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Subject");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771B3A5A257A");

            entity.ToTable("Enrollment");

            entity.Property(e => e.EnrollDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Student");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301DDAEB7F34E");

            entity.ToTable("Semester");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterName).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52B99650CBD06");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UQ__Student__A9D10534714C397F").IsUnique();

            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__AC1BA3A8CF214707");

            entity.ToTable("Subject");

            entity.HasIndex(e => e.SubjectCode, "UQ__Subject__9F7CE1A96F28DEE5").IsUnique();

            entity.Property(e => e.SubjectCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
