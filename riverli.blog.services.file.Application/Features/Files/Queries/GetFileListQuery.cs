using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;

namespace riverli.blog.services.file.Application.Features.Files.Queries;

/// <summary>
/// 分页查询文件列表
/// </summary>
public record GetFileListQuery : IRequest<PagedResult<FileDto>>
{
    public Guid? DirectoryId { get; init; }
    public string? Keyword { get; init; }
    public string? ContentType { get; init; }
    public Guid? TagId { get; init; }
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
