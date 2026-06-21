namespace riverli.blog.services.file.Domain.Enums;

/// <summary>
/// 文件存储类型
/// </summary>
public enum FileStorageType
{
    /// <summary>
    /// 本地存储
    /// </summary>
    Local = 1,

    /// <summary>
    /// 阿里云 OSS
    /// </summary>
    AliyunOss = 2,

    /// <summary>
    /// 腾讯云 COS
    /// </summary>
    TencentCos = 3,

    /// <summary>
    /// AWS S3
    /// </summary>
    AwsS3 = 4,

    /// <summary>
    /// MinIO
    /// </summary>
    Minio = 5
}
