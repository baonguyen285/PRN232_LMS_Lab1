using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DB Context with SQL Server
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register Generic Repository and Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

// 3. Add Controllers and JSON configurations (to prevent potential cycle issues, though DTOs prevent them)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// 4. Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PRN232 - Lab 1 LMS API",
        Version = "v1",
        Description = "RESTful API built with 3-layer architecture for Learning Management System (LMS)."
    });
});

var app = builder.Build();

// 5. Automatic Database Initialization & Seeding on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
        // EnsureCreated creates DB and runs the HasData seeding configured in OnModelCreating
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating or seeding the database.");
    }
}

// 6. Configure HTTP pipeline
if (app.Environment.IsDevelopment() || true) // Enable Swagger in both Dev & Prod for grading convenience
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 Lab 1 API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the application's root URL (localhost:port/)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

