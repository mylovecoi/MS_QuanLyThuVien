using FileService.API.Extensions;
using FileService.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Configuration;
using Shared.Contracts.Extensions;
using Shared.Contracts.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Fail-fast nếu secret chưa được cấu hình
builder.Configuration.RequireSecrets(
    "ConnectionStrings:DefaultConnection",
    "Jwt:SecretKey",
    "InternalApi:ApiKey");


// 1. Add Services to DI container
builder.Services.AddControllers();
builder.Services.AddApiResponseConventions();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Custom Extension Methods
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddRepositoriesAndServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Tự động apply migration khi khởi động
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FileDbContext>();
    await context.Database.MigrateAsync();
}

// 2. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");
app.UseStaticFiles(); // Phục vụ file tĩnh nếu truy cập trực tiếp qua static file URL

app.UseApiResponseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();
