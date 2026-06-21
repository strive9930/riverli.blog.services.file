using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

/// <summary>
/// 删除文件命令
/// </summary>
public record DeleteFileCommand : IRequest<Result>
{
    public Guid FileId { get; init; }
}
