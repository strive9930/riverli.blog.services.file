using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;

namespace riverli.blog.services.file.Application.Features.Directories.Queries;

/// <summary>
/// 获取目录树
/// </summary>
public record GetDirectoryTreeQuery : IRequest<Result<List<FileDirectoryDto>>>;
