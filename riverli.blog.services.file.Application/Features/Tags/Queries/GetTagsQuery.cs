using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;

namespace riverli.blog.services.file.Application.Features.Tags.Queries;

/// <summary>
/// 获取所有标签
/// </summary>
public record GetTagsQuery : IRequest<Result<List<FileTagDto>>>;
