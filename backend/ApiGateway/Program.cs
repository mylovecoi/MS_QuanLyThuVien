var builder = WebApplication.CreateBuilder(args);

// ==============================
// 1. Đăng ký YARP Reverse Proxy (đọc cấu hình từ appsettings.json)
// ==============================
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ==============================
// 2. CORS: Cho phép React App gọi vào Gateway
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ==============================
// 3. Middleware Pipeline
// ==============================
app.UseCors("AllowReactApp");

// Map YARP Reverse Proxy routes
app.MapReverseProxy();

app.Run();
