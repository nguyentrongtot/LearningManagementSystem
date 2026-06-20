
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.API.Validators;
using PRN232.LMS.Repositories.DAL;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.BLL;
using PRN232.LMS.Services.Business;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(entry => entry.Value?.Errors.Count > 0)
                            .SelectMany(entry => entry.Value!.Errors.Select(error => error.ErrorMessage))
                            .ToList();

                        return new BadRequestObjectResult(new ApiErrorResponse
                        {
                            Success = false,
                            Message = "Validation failed",
                            Errors = errors
                        });
                    };
                });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            
            builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
            builder.Services.AddScoped<ISemesterService, SemesterService>();
            
            builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("Jwt settings are missing.");
            builder.Services.AddSingleton(jwtSettings);
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

            builder.Services.AddDbContext<LmsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
                DbInitializer.InitializeAsync(context).GetAwaiter().GetResult();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            // Configure the HTTP request pipeline.


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            
            app.Run();
        }
    }
}
