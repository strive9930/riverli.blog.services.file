using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Infrastructure.Shared.Extensions;
using RiverLi.Blog.Infrastructure.Shared.Repositories;
using RiverLi.Blog.Infrastructure.Shared.Security;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Infrastructure.Data;
using riverli.blog.services.file.Infrastructure.Repositories;
using riverli.blog.services.file.Infrastructure.Storage;
using RiverLi.DDD.Core.Domain.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. 【共享基建注入】
// ==========================================

builder.Services.AddControllers();

builder.Services.AddLoggingSupport(builder.Configuration, "FileService");

builder.Services.AddInfrastructureSharedServices(options =>
{
    options.EnableGlobalExceptionHandler = true;
    options.EnableCors = true;
    options.AllowedOrigins = new[] { "http://localhost:5000", "http://localhost:3000", "http://localhost:5173" };
    options.EnableOpenApiDocumentation = true;
    options.ScalarTitle = "RiverLi Blog File API";
    options.ScalarVersion = "v1";
    options.ScalarDescription = "文件管理微服务接口文档";
});

builder.Services.AddHealthCheckSupport(builder.Configuration);

// ==========================================
// 2. 【业务专属注入】
// ==========================================

// 数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("未获取到连接字符串");
builder.Services.AddDbContext<FileDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 将 FileDbContext 注册为 RiverDbContext（共享库需要）
builder.Services.AddScoped<RiverLi.Blog.Infrastructure.Shared.Data.RiverDbContext>(
    sp => sp.GetRequiredService<FileDbContext>());

// 认证授权
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(riverli.blog.services.file.Application.Features.Files.Commands.UploadFileCommand).Assembly));

// 仓储
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileDirectoryRepository, FileDirectoryRepository>();
builder.Services.AddScoped<IFileTagRepository, FileTagRepository>();

// 通用仓储（用于共享库内默认实现）
builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

// 存储服务
builder.Services.Configure<LocalStorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddScoped<IStorageService, LocalStorageService>();

// 全局授权过滤器
builder.Services.AddScoped<GlobalApiAuthorizeFilter>();
builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
{
    options.Filters.AddService<GlobalApiAuthorizeFilter>();
});

// Memory Cache
builder.Services.AddMemoryCache();

// OpenAPI 自报告
builder.Services.AddApiSelfReporting();
builder.Services.AddMicroserviceOpenApi();

// ==========================================
// 3. 【中间件管道配置】
// ==========================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("RiverLi Blog - 文件管理微服务")
            .WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseInfrastructureSharedMiddlewares();
await DbSeeder.SeedAsync(app.Services);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 静态文件（用于本地存储访问）
app.UseStaticFiles();

app.MapInfrastructureSharedEndpoints(options =>
{
    options.EnableOpenApiDocumentation = app.Environment.IsDevelopment();
    options.ScalarTitle = "RiverLi Blog File API";
});

app.Run();
