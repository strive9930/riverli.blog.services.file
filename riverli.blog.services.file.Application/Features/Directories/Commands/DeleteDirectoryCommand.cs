using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Directories.Commands;

/// <summary>
/// 删除目录命令
/// </summary>
public record DeleteDirectoryCommand : IRequest<Result>
{
    public Guid DirectoryId { get; init; }
}
