using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using riverli.blog.services.file.Application.Constants;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Infrastructure.Storage;

/// <summary>
/// 本地文件存储服务
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly LocalStorageOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(
        IOptions<LocalStorageOptions> options,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LocalStorageService> logger)
    {
        _options = options.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<StorageResult> UploadAsync(
        string fileName, string contentType, Stream stream, string bucket,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 生成唯一存储名
            var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
            var storedName = $"{Guid.NewGuid():N}.{extension}";

            // 按日期分目录存储
            var datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var relativePath = Path.Combine(bucket, datePath, storedName);
            var fullPath = Path.Combine(_options.RootPath, relativePath);

            // 确保目录存在
            var directory = Path.GetDirectoryName(fullPath)!;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // 写入文件
            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream, cancellationToken);

            var fileSize = new FileInfo(fullPath).Length;
            var url = GetFileUrl(relativePath);

            // 图片类文件尝试获取尺寸和缩略图（需要 SkiaSharp/SixLabors.ImageSharp）
            int? width = null, height = null;
            string? thumbnailUrl = null;

            _logger.LogInformation("文件已存储: {StoredName}, 路径: {RelativePath}, 大小: {FileSize}",
                storedName, relativePath, fileSize);

            return StorageResult.Ok(storedName, relativePath, url, fileSize, width, height, thumbnailUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件存储失败: {FileName}", fileName);
            return StorageResult.Fail(ex.Message);
        }
    }

    public Task<Stream?> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_options.RootPath, storagePath);
        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_options.RootPath, storagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("文件已删除: {StoragePath}", storagePath);
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件删除失败: {StoragePath}", storagePath);
            return Task.FromResult(false);
        }
    }

    public string GetFileUrl(string storagePath)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request is null)
            return $"/files/{storagePath.Replace('\\', '/')}";

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{_options.RequestPath.Trim('/')}/{storagePath.Replace('\\', '/')}";
    }

    public async Task<StorageResult?> GenerateThumbnailAsync(
        string storagePath, int maxWidth = 200, int maxHeight = 200,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_options.RootPath, storagePath);
            if (!File.Exists(fullPath)) return null;

            var extension = Path.GetExtension(storagePath).TrimStart('.').ToLowerInvariant();
            if (extension is "svg+xml" or "gif") return null; // 跳过 SVG 和 GIF

            // 使用 SkiaSharp 生成缩略图（需要安装 SkiaSharp NuGet 包）
            // 简化实现：复制并标记为 thumb
            var dir = Path.GetDirectoryName(storagePath)!;
            var fileName = Path.GetFileNameWithoutExtension(storagePath);
            var thumbName = $"{FileConstants.ThumbnailPrefix}{fileName}.{extension}";
            var thumbRelativePath = Path.Combine(dir, thumbName);
            var thumbFullPath = Path.Combine(_options.RootPath, thumbRelativePath);

            // 简单复制（实际项目应使用图片处理库如 SkiaSharp/SixLabors.ImageSharp）
            await using var sourceStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            await using var destStream = new FileStream(thumbFullPath, FileMode.Create, FileAccess.Write);
            await sourceStream.CopyToAsync(destStream, cancellationToken);

            return StorageResult.Ok(thumbName, thumbRelativePath, GetFileUrl(thumbRelativePath),
                new FileInfo(thumbFullPath).Length);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "缩略图生成失败: {StoragePath}", storagePath);
            return null;
        }
    }

    /// <summary>
    /// 本地存储配置
    /// </summary>
    public class LocalStorageOptions
    {
        /// <summary>
        /// 文件存储根路径
        /// </summary>
        public string RootPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        /// <summary>
        /// HTTP 请求路径前缀
        /// </summary>
        public string RequestPath { get; set; } = "uploads";
    }
}
