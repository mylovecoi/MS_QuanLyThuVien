using Microsoft.EntityFrameworkCore;
using ProfileService.API.Extensions;
using ProfileService.DataAccess.Contexts;
using ProfileService.DataAccess.Seed;
using Shared.Contracts.Configuration;
using Shared.Contracts.Constants;
using Shared.Contracts.Extensions;
using Shared.Contracts.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Fail-fast nếu secret chưa được cấu hình, thay vì chạy tiếp với khoá rỗng.
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
builder.Services.AddRepositoriesAndServices();
builder.Services.AddInternalApiAuthentication(builder.Configuration);
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

// Tự động apply migration khi khởi động (tạo DB/table nếu chưa có, cập nhật schema nếu có migration mới)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
    await context.Database.MigrateAsync();
    await ProfileDbSeeder.SeedDataAsync(context, SeedConstants.AdminUserId);
}

// 2. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORS phải đứng trước mọi middleware có thể short-circuit và ghi body,
// nếu không response lỗi sẽ thiếu Access-Control-Allow-Origin và browser chặn JS đọc.
app.UseCors("AllowReactApp");

// Đặt trước UseAuthentication: JwtBearerHandler challenge sẽ short-circuit
// nên các middleware phía dưới (kể cả UseStatusCodePages) không chạy được.
// Điều này đảm bảo 401/403 cũng có envelope ApiResponse.
app.UseApiResponseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Đặt trong MapControllers để vẫn bắt được exception phát sinh từ controller,
// nhưng sau UseCors nên body lỗi luôn có header CORS.
app.UseMiddleware<ExceptionMiddleware>();

app.Run();