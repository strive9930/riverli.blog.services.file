namespace riverli.blog.services.file.Application.DTOs;

/// <summary>
/// 文件标签 DTO
/// </summary>
public class FileTagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#1890FF";
    public int FileCount { get; set; }
    public DateTime CreateTime { get; set; }
}
