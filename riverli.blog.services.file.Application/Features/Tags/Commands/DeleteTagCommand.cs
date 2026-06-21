using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace riverli.blog.services.file.Application.Features.Tags.Commands;

/// <summary>
/// 删除标签命令
/// </summary>
public record DeleteTagCommand : IRequest<Result>
{
    public Guid TagId { get; init; }
}
