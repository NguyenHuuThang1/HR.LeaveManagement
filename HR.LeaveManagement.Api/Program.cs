using HR.LeaveManagement.Api.Middleware;
using HR.LeaveManagement.Application;
using HR.LeaveManagement.Identity;
using HR.LeaveManagement.Infrastructure;
using HR.LeaveManagement.Persistence;
using Microsoft.OpenApi.Models;

// 1. KHỞI TẠO BUILDER
var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 2. ĐĂNG KÝ DỊCH VỤ (Thay thế cho ConfigureServices cũ)
// ==========================================================
builder.Services.AddHttpContextAccessor();

// Cấu hình Swagger trực tiếp
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HR Leave Management Api",
    });
});

// Đăng ký các tầng trong Clean Architecture
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);

builder.Services.AddControllers();

// Cấu hình CORS
builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy",
        corsBuilder => corsBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// ==========================================================
// 3. CẤU HÌNH PIPELINE (Thay thế cho Configure cũ)
// ==========================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Gọi Middleware xử lý lỗi tự tạo của dự án
app.UseMiddleware<ExceptionMiddleware>();

// Thứ tự các Use này rất quan trọng, không nên đảo lộn
app.UseAuthentication();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR.LeaveManagement.Api v1"));

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

// Chuẩn mới của .NET để map các Endpoints/Controllers
app.MapControllers();

// 4. CHẠY ỨNG DỤNG
app.Run();