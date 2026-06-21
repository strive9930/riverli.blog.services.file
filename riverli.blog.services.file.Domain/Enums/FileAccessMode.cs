namespace riverli.blog.services.file.Domain.Enums;

/// <summary>
/// 文件访问模式
/// </summary>
public enum FileAccessMode
{
    /// <summary>
    /// 公开访问
    /// </summary>
    Public = 1,

    /// <summary>
    /// 登录后可访问
    /// </summary>
    Protected = 2,

    /// <summary>
    /// 仅拥有者可访问
    /// </summary>
    Private = 3
}
