using RiverLi.DDD.Core.Domain.Common;

namespace riverli.blog.services.file.Domain.Entities;

/// <summary>
/// 文件目录实体
/// </summary>
public class FileDirectory : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// 目录名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父目录 ID（null 表示根目录）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 父目录导航属性
    /// </summary>
    public FileDirectory? Parent { get; set; }

    /// <summary>
    /// 子目录集合
    /// </summary>
    public ICollection<FileDirectory> Children { get; set; } = new List<FileDirectory>();

    /// <summary>
    /// 目录下文件集合
    /// </summary>
    public ICollection<FileItem> Files { get; set; } = new List<FileItem>();

    /// <summary>
    /// 完整路径（如 /docs/images/）
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 目录描述
    /// </summary>
    public string? Description { get; set; }

    public FileDirectory()
    {
        CreateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新目录信息
    /// </summary>
    public void Update(string name, string? description, int? sortOrder)
    {
        Name = name;
        if (description is not null) Description = description;
        if (sortOrder.HasValue) SortOrder = sortOrder.Value;
        UpdateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 重新计算完整路径
    /// </summary>
    public void RecalculatePath(string? parentPath)
    {
        Path = string.IsNullOrEmpty(parentPath) ? $"/{Name}/" : $"{parentPath.TrimEnd('/')}/{Name}/";
    }
}
