namespace riverli.blog.services.file.Application.Interfaces;

/// <summary>
/// 文件存储服务抽象（支持本地、OSS、S3 等多种后端）
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// 上传文件流并返回存储信息
    /// </summary>
    /// <param name="fileName">原始文件名</param>
    /// <param name="contentType">MIME 类型</param>
    /// <param name="stream">文件流</param>
    /// <param name="bucket">存储桶/分类目录</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存储结果（含存储路径、URL、大小等）</returns>
    Task<StorageResult> UploadAsync(string fileName, string contentType, Stream stream, string bucket, CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载文件流
    /// </summary>
    Task<Stream?> DownloadAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文件
    /// </summary>
    Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件访问 URL
    /// </summary>
    string GetFileUrl(string storagePath);

    /// <summary>
    /// 生成缩略图并上传（仅图片）
    /// </summary>
    Task<StorageResult?> GenerateThumbnailAsync(string storagePath, int maxWidth = 200, int maxHeight = 200, CancellationToken cancellationToken = default);
}

/// <summary>
/// 存储操作结果
/// </summary>
public class StorageResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 存储名（唯一标识）
    /// </summary>
    public string StoredName { get; set; } = string.Empty;

    /// <summary>
    /// 存储相对路径
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 访问 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 图片宽度（仅图片）
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 图片高度（仅图片）
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// 缩略图 URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    public static StorageResult Ok(string storedName, string storagePath, string url, long fileSize, int? width = null, int? height = null, string? thumbnailUrl = null)
        => new()
        {
            Success = true,
            StoredName = storedName,
            StoragePath = storagePath,
            Url = url,
            FileSize = fileSize,
            Width = width,
            Height = height,
            ThumbnailUrl = thumbnailUrl
        };

    public static StorageResult Fail(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}
