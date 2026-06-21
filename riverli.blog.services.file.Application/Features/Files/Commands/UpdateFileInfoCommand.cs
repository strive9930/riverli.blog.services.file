using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

/// <summary>
/// 更新文件信息命令
/// </summary>
public record UpdateFileInfoCommand : IRequest<Result>
{
    public Guid FileId { get; init; }
    public string? FileName { get; init; }
    public string? Description { get; init; }
    public bool? IsPublic { get; init; }
    public Guid? DirectoryId { get; init; }
    public List<Guid>? TagIds { get; init; }
}
