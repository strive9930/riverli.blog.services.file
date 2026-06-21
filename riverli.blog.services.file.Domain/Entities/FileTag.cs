using RiverLi.DDD.Core.Domain.Common;

namespace riverli.blog.services.file.Domain.Entities;

/// <summary>
/// 文件标签实体
/// </summary>
public class FileTag : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// 标签名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 标签颜色（十六进制，如 #FF5733）
    /// </summary>
    public string Color { get; set; } = "#1890FF";

    /// <summary>
    /// 关联的文件集合（多对多）
    /// </summary>
    public ICollection<FileItem> Files { get; set; } = new List<FileItem>();

    public FileTag()
    {
        CreateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新标签
    /// </summary>
    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
    }
}
