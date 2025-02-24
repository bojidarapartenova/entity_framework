using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data
{
    using System.Reflection.Emit;
    using P01_StudentSystem.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Homework> Homeworks { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentCourse> StudentsCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=StudentSystem;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<StudentCourse>()
                .HasKey(s => new { s.StudentId, s.CourseId });
        }
    }
}
