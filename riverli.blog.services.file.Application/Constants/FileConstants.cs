namespace riverli.blog.services.file.Application.Constants;

/// <summary>
/// 文件服务常量
/// </summary>
public static class FileConstants
{
    /// <summary>
    /// 最大文件大小（50MB）
    /// </summary>
    public const long MaxFileSize = 50 * 1024 * 1024;

    /// <summary>
    /// 支持的图片 MIME 类型
    /// </summary>
    public static readonly string[] ImageContentTypes = ["image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp", "image/svg+xml"];

    /// <summary>
    /// 支持的文档 MIME 类型
    /// </summary>
    public static readonly string[] DocumentContentTypes = [
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "text/markdown"
    ];

    /// <summary>
    /// 默认存储桶
    /// </summary>
    public const string DefaultBucket = "files";

    /// <summary>
    /// 图片存储桶
    /// </summary>
    public const string ImageBucket = "images";

    /// <summary>
    /// 缩略图前缀
    /// </summary>
    public const string ThumbnailPrefix = "thumb_";

    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public const string CacheKeyPrefix = "file_service:";
}
