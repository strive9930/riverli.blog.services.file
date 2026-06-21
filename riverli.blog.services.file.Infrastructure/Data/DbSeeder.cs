using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace riverli.blog.services.file.Infrastructure.Data;

/// <summary>
/// 数据库初始化
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FileDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("FileDbSeeder");

        try
        {
            logger.LogInformation("正在迁移文件服务数据库...");
            await context.Database.MigrateAsync();
            logger.LogInformation("文件服务数据库迁移完成");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "文件服务数据库迁移失败");
            throw;
        }
    }
}
