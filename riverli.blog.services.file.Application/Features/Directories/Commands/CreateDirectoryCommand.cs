using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Directories.Commands;

/// <summary>
/// 创建目录命令
/// </summary>
public record CreateDirectoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public string? Description { get; init; }
}
