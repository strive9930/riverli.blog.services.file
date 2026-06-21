namespace riverli.blog.services.file.Application.DTOs;

/// <summary>
/// 文件目录 DTO（树形结构）
/// </summary>
public class FileDirectoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string Path { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Description { get; set; }
    public int FileCount { get; set; }
    public List<FileDirectoryDto> Children { get; set; } = new();
    public DateTime CreateTime { get; set; }
    public string? Creator { get; set; }
    public DateTime? UpdateTime { get; set; }
}
