using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Context
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configure Semester
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.SemesterId);
                entity.Property(e => e.SemesterName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.StartDate).HasColumnType("datetime").IsRequired();
                entity.Property(e => e.EndDate).HasColumnType("datetime").IsRequired();
            });

            // 2. Configure Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseName).HasMaxLength(100).IsRequired();

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Configure Subject
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.SubjectId);
                entity.Property(e => e.SubjectCode).HasMaxLength(20).IsUnicode(false).IsRequired();
                entity.Property(e => e.SubjectName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Credit).IsRequired();
            });

            // 4. Configure Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).IsRequired();
                entity.Property(e => e.DateOfBirth).HasColumnType("datetime").IsRequired();
            });

            // 5. Configure Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.Property(e => e.EnrollDate).HasColumnType("datetime").IsRequired();
                entity.Property(e => e.Status).HasMaxLength(20).IsUnicode(false).IsRequired();

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Database Seeding ---

            // Seed Semesters (5 semesters)
            var semesters = new List<Semester>();
            var baseDate = new DateTime(2025, 1, 1);
            for (int i = 1; i <= 5; i++)
            {
                semesters.Add(new Semester
                {
                    SemesterId = i,
                    SemesterName = $"Semester {i} - {(i % 2 == 1 ? "Fall" : "Spring")} {baseDate.Year + (i - 1) / 2}",
                    StartDate = baseDate.AddMonths((i - 1) * 6),
                    EndDate = baseDate.AddMonths((i - 1) * 6 + 5)
                });
            }
            modelBuilder.Entity<Semester>().HasData(semesters);

            // Seed Subjects (10 subjects)
            var subjects = new List<Subject>
            {
                new Subject { SubjectId = 1, SubjectCode = "PRN211", SubjectName = "Basic Cross-Platform Application Programming", Credit = 3 },
                new Subject { SubjectId = 2, SubjectCode = "PRN221", SubjectName = "Advanced Cross-Platform Application Programming", Credit = 3 },
                new Subject { SubjectId = 3, SubjectCode = "PRN231", SubjectName = "Web API Development", Credit = 3 },
                new Subject { SubjectId = 4, SubjectCode = "PRN232", SubjectName = "Advanced Web API and Deployment", Credit = 3 },
                new Subject { SubjectId = 5, SubjectCode = "PRG191", SubjectName = "Core Java Programming", Credit = 3 },
                new Subject { SubjectId = 6, SubjectCode = "PRU211", SubjectName = "C# and Unity Game Development", Credit = 3 },
                // Extra subjects to make 10
                new Subject { SubjectId = 7, SubjectCode = "SWD392", SubjectName = "Software Architecture and Design", Credit = 3 },
                new Subject { SubjectId = 8, SubjectCode = "SWE201", SubjectName = "Software Engineering Introduction", Credit = 3 },
                new Subject { SubjectId = 9, SubjectCode = "DBI202", SubjectName = "Introduction to Databases", Credit = 3 },
                new Subject { SubjectId = 10, SubjectCode = "PRJ301", SubjectName = "Java Web Application Development", Credit = 3 }
            };
            modelBuilder.Entity<Subject>().HasData(subjects);

            // Seed Courses (20 courses)
            var courses = new List<Course>();
            for (int i = 1; i <= 20; i++)
            {
                int semId = ((i - 1) % 5) + 1; // 1 to 5
                courses.Add(new Course
                {
                    CourseId = i,
                    CourseName = $"Course {i} - {subjects[(i - 1) % 10].SubjectCode} (Class SE18{(10 + i)})",
                    SemesterId = semId
                });
            }
            modelBuilder.Entity<Course>().HasData(courses);

            // Seed Students (50 students)
            var students = new List<Student>();
            string[] firstNames = { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
            string[] middleNames = { "Van", "Thi", "Anh", "Minh", "Quoc", "Duc", "Gia", "Thanh", "Hoang", "Kim" };
            string[] lastNames = { "Bao", "An", "Binh", "Dung", "Hai", "Hung", "Linh", "Nam", "Phong", "Vy" };
            
            for (int i = 1; i <= 50; i++)
            {
                string fn = firstNames[(i - 1) % 10];
                string mn = middleNames[(i * 3) % 10];
                string ln = lastNames[(i * 7) % 10];
                string fullName = $"{fn} {mn} {ln}";
                string email = $"{ln.ToLower()}.{fn.ToLower()}{i}@fpt.edu.vn";

                students.Add(new Student
                {
                    StudentId = i,
                    FullName = fullName,
                    Email = email,
                    DateOfBirth = new DateTime(2004, 1, 1).AddDays(i * 15)
                });
            }
            modelBuilder.Entity<Student>().HasData(students);

            // Seed Enrollments (500 enrollments)
            // We have 50 students and 20 courses. We can enroll each student in 10 different courses.
            // 50 students * 10 courses = 500 enrollments!
            var enrollments = new List<Enrollment>();
            int enrollmentId = 1;
            string[] statuses = { "Active", "Completed", "Dropped" };

            for (int studentId = 1; studentId <= 50; studentId++)
            {
                // Each student registers 10 courses starting from (studentId % 20)
                for (int cOffset = 0; cOffset < 10; cOffset++)
                {
                    int courseId = ((studentId + cOffset) % 20) + 1; // 1 to 20
                    
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = enrollmentId,
                        StudentId = studentId,
                        CourseId = courseId,
                        EnrollDate = new DateTime(2025, 2, 1).AddDays(enrollmentId % 20),
                        Status = statuses[enrollmentId % 3] // Cycles between Active, Completed, Dropped
                    });
                    enrollmentId++;
                }
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);
        }
    }
}

