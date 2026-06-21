using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Tags.Commands;

/// <summary>
/// 创建标签命令
/// </summary>
public record CreateTagCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#1890FF";
}
