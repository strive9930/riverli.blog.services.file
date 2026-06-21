using RiverLi.DDD.Core.Domain.Common;

namespace riverli.blog.services.file.Domain.Entities;

/// <summary>
/// 文件实体 - 核心聚合根
/// </summary>
public class FileItem : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// 原始文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 存储后的唯一文件名
    /// </summary>
    public string StoredName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名（不含点）
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// MIME 类型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 存储桶/根目录路径
    /// </summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// 存储相对路径（含存储名）
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// 访问 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图 URL（图片类文件才有）
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 所属目录 ID（null 表示根目录）
    /// </summary>
    public Guid? DirectoryId { get; set; }

    /// <summary>
    /// 所属目录导航属性
    /// </summary>
    public FileDirectory? Directory { get; set; }

    /// <summary>
    /// 图片宽度（像素，仅图片类文件）
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 图片高度（像素，仅图片类文件）
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// 文件标签（多对多）
    /// </summary>
    public ICollection<FileTag> Tags { get; set; } = new List<FileTag>();

    /// <summary>
    /// 是否公开访问
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// 文件描述
    /// </summary>
    public string? Description { get; set; }

    public FileItem()
    {
        CreateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新文件基本信息
    /// </summary>
    public void UpdateInfo(string fileName, string? description, bool? isPublic)
    {
        FileName = fileName;
        if (description is not null) Description = description;
        if (isPublic.HasValue) IsPublic = isPublic.Value;
        UpdateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 移动到指定目录
    /// </summary>
    public void MoveTo(Guid? directoryId)
    {
        DirectoryId = directoryId;
        UpdateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 增加下载计数
    /// </summary>
    public void IncrementDownload()
    {
        DownloadCount++;
    }

    /// <summary>
    /// 添加标签
    /// </summary>
    public void AddTag(FileTag tag)
    {
        if (!Tags.Any(t => t.Id == tag.Id))
            Tags.Add(tag);
    }

    /// <summary>
    /// 移除标签
    /// </summary>
    public void RemoveTag(Guid tagId)
    {
        var tag = Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
            Tags.Remove(tag);
    }
}
