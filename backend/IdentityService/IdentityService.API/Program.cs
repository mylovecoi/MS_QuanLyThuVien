using Microsoft.EntityFrameworkCore;
using IdentityService.API.Extensions;
using IdentityService.API.Filters;
using IdentityService.API.Middlewares;
using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Seed;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services to DI container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Custom Extension Methods
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddRepositoriesAndServices();
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

// Tự động khởi tạo Database khi chưa có, dùng lại nếu đã có
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedDataAsync(context);
}

// 2. Configure HTTP Request Pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
