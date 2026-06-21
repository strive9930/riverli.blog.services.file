using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;

namespace riverli.blog.services.file.Application.Features.Files.Queries;

/// <summary>
/// 获取单个文件
/// </summary>
public record GetFileQuery : IRequest<Result<FileDto>>
{
    public Guid FileId { get; init; }
}
