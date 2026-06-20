using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(LmsDbContext context)
    {
        await EnsureDatabaseMigratedAsync(context);
        await SeedUsersAsync(context);

        if (await context.Students.AnyAsync())
        {
            return;
        }

        var semesters = new[]
        {
            new Semester { SemesterName = "Spring2024", StartDate = new DateTime(2024, 1, 8), EndDate = new DateTime(2024, 3, 26) },
            new Semester { SemesterName = "Summer2024", StartDate = new DateTime(2024, 5, 12), EndDate = new DateTime(2024, 7, 31) },
            new Semester { SemesterName = "Fall2024", StartDate = new DateTime(2024, 9, 16), EndDate = new DateTime(2024, 11, 25) },
            new Semester { SemesterName = "Spring2025", StartDate = new DateTime(2025, 1, 8), EndDate = new DateTime(2025, 3, 26) },
            new Semester { SemesterName = "Summer2025", StartDate = new DateTime(2025, 5, 12), EndDate = new DateTime(2025, 7, 31) }
        };
        context.Semesters.AddRange(semesters);
        await context.SaveChangesAsync();

        var subjects = new[]
        {
            new Subject { SubjectCode = "PRF192", SubjectName = "Programming Fundamentals", Credit = 3 },
            new Subject { SubjectCode = "MAE101", SubjectName = "Mathematics for Engineering", Credit = 3 },
            new Subject { SubjectCode = "CEA201", SubjectName = "Computer Organization and Architecture", Credit = 3 },
            new Subject { SubjectCode = "SSL101", SubjectName = "Academic Skills for University Success", Credit = 3 },
            new Subject { SubjectCode = "CSI104", SubjectName = "Introduction to Computing", Credit = 3 },
            new Subject { SubjectCode = "NWC204", SubjectName = "Computer Networking", Credit = 3 },
            new Subject { SubjectCode = "SSG104", SubjectName = "Communication and In-Group Working Skills", Credit = 3 },
            new Subject { SubjectCode = "PRO192", SubjectName = "Object-Oriented Programming", Credit = 3 },
            new Subject { SubjectCode = "MAD101", SubjectName = "Discrete Mathematics", Credit = 3 },
            new Subject { SubjectCode = "OSG202", SubjectName = "Operating Systems", Credit = 3 }
        };
        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        var students = new List<Student>();
        var firstNames = new[]
        {
            "Nguyen Van", "Tran Duc", "Le Minh", "Pham Hoang", "Nguyen Thi",
            "Vo Thanh", "Bui Quoc", "Do Van", "Tran Minh", "Nguyen Quang"
        };
        var lastNames = new[] { "An", "Binh", "Cuong", "Dung", "Em", "Giang", "Hieu", "Hung", "Khanh", "Lam" };
        var emailPrefixes = new[] { "nva", "tdb", "lmc", "phd", "nte", "vtg", "bqh", "dvh", "tmk", "nql" };

        for (var i = 1; i <= 50; i++)
        {
            var fn = firstNames[(i - 1) % firstNames.Length];
            var ln = lastNames[(i - 1) % lastNames.Length];

            students.Add(new Student
            {
                FullName = $"{fn} {ln}",
                Email = $"{emailPrefixes[(i - 1) % emailPrefixes.Length]}se{180000 + i:D6}@fpt.edu.vn",
                DateOfBirth = new DateTime(2003 + (i % 2), (i % 12) + 1, (i % 28) + 1)
            });
        }

        students[0].FullName = "Nguyen Van An";
        students[0].Email = "nvase180001@fpt.edu.vn";
        students[0].DateOfBirth = new DateTime(2004, 1, 15);

        context.Students.AddRange(students);
        await context.SaveChangesAsync();

        var courses = new List<Course>();
        for (var i = 0; i < 20; i++)
        {
            var semesterId = (i % 5) + 1;
            var subjectId = (i % 10) + 1;
            courses.Add(new Course
            {
                CourseName = $"{subjects[subjectId - 1].SubjectCode}_SE{(1700 + i + 1)}",
                SemesterId = semesterId,
                SubjectId = subjectId
            });
        }
        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        var statuses = new[] { "Active", "Dropped", "Completed" };
        var enrollments = new List<Enrollment>();
        var enrollDate = new DateTime(2024, 1, 10);
        var enrollmentId = 0;

        while (enrollments.Count < 500)
        {
            foreach (var student in students)
            {
                foreach (var course in courses)
                {
                    if (enrollments.Count >= 500)
                    {
                        break;
                    }

                    enrollments.Add(new Enrollment
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        EnrollDate = enrollDate.AddDays(enrollmentId % 365),
                        Status = statuses[enrollmentId % statuses.Length]
                    });
                    enrollmentId++;
                }

                if (enrollments.Count >= 500)
                {
                    break;
                }
            }
        }

        context.Enrollments.AddRange(enrollments);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(LmsDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = new[]
        {
            new User
            {
                Username = "admin",
                PasswordHash = HashPassword("123456"),
                Role = "Admin"
            },
            new User
            {
                Username = "totntse",
                PasswordHash = HashPassword("123456"),
                Role = "Student"
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureDatabaseMigratedAsync(LmsDbContext context)
    {
        const int maxRetries = 15;
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await ApplyMigrationsAsync(context);
                return;
            }
            catch (Exception ex) when (attempt < maxRetries && IsTransientDbError(ex))
            {
                lastException = ex;
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        throw new InvalidOperationException(
            "Cannot connect to the database. Check your connection string.",
            lastException);
    }

    private static async Task ApplyMigrationsAsync(LmsDbContext context)
    {
        List<string> pendingMigrations;
        try
        {
            pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
        }
        catch (SqlException ex) when (ex.Number == 4060)
        {
            await context.Database.MigrateAsync();
            return;
        }

        if (pendingMigrations.Count == 0)
        {
            return;
        }

        if (!await context.Database.CanConnectAsync())
        {
            await context.Database.MigrateAsync();
            return;
        }

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (SqlException ex) when (ex.Number == 2714)
        {
            await BaselineMigrationsAsync(context, pendingMigrations);
        }
    }

    private static bool IsTransientDbError(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is SqlException sqlEx)
            {
                return sqlEx.Number is -2 or 4060 or 18456 or 233 or 10061;
            }
        }

        return ex is TimeoutException;
    }

    private static async Task<bool> TableExistsAsync(LmsDbContext context, string tableName)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN OBJECT_ID(@tableName, 'U') IS NOT NULL THEN 1 ELSE 0 END";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) == 1;
    }

    private static async Task BaselineMigrationsAsync(LmsDbContext context, IEnumerable<string> migrationIds)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
            CREATE TABLE [__EFMigrationsHistory] (
                [MigrationId] nvarchar(150) NOT NULL,
                [ProductVersion] nvarchar(32) NOT NULL,
                CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
            );
            """);

        foreach (var migrationId in migrationIds)
        {
            await context.Database.ExecuteSqlInterpolatedAsync($"""
                IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {migrationId})
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ({migrationId}, {"8.0.1"});
                """);
        }
    }

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);
}
