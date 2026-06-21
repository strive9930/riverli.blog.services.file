using MediatR;
using Microsoft.AspNetCore.Http;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

/// <summary>
/// 上传文件命令
/// </summary>
public record UploadFileCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// 上传的文件
    /// </summary>
    public IFormFile File { get; init; } = null!;

    /// <summary>
    /// 目标目录 ID（可选）
    /// </summary>
    public Guid? DirectoryId { get; init; }

    /// <summary>
    /// 存储桶（可选，默认自动判断）
    /// </summary>
    public string? Bucket { get; init; }

    /// <summary>
    /// 是否公开
    /// </summary>
    public bool IsPublic { get; init; } = true;

    /// <summary>
    /// 文件描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 标签 ID 列表
    /// </summary>
    public List<Guid>? TagIds { get; init; }
}
