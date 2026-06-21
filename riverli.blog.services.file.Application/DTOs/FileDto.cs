namespace riverli.blog.services.file.Application.DTOs;

/// <summary>
/// 文件 DTO
/// </summary>
public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoredName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeFormatted => FormatFileSize(FileSize);
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public Guid? DirectoryId { get; set; }
    public string? DirectoryName { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsPublic { get; set; }
    public int DownloadCount { get; set; }
    public string? Description { get; set; }
    public List<FileTagDto> Tags { get; set; } = new();
    public DateTime CreateTime { get; set; }
    public string? Creator { get; set; }
    public DateTime? UpdateTime { get; set; }

    private static string FormatFileSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
            < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
            _ => $"{bytes / (1024.0 * 1024 * 1024):F2} GB"
        };
    }
}
