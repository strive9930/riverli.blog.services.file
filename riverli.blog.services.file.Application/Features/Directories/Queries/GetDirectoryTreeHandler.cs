using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Directories.Queries;

public class GetDirectoryTreeHandler : IRequestHandler<GetDirectoryTreeQuery, Result<List<FileDirectoryDto>>>
{
    private readonly IFileDirectoryRepository _directoryRepository;

    public GetDirectoryTreeHandler(IFileDirectoryRepository directoryRepository)
    {
        _directoryRepository = directoryRepository;
    }

    public async Task<Result<List<FileDirectoryDto>>> Handle(GetDirectoryTreeQuery request, CancellationToken cancellationToken)
    {
        var directories = await _directoryRepository.GetTreeAsync(cancellationToken);
        var dtos = directories.Select(MapToTree).ToList();
        return Result<List<FileDirectoryDto>>.SuccessResult(dtos);
    }

    private static FileDirectoryDto MapToTree(Domain.Entities.FileDirectory dir)
    {
        return new FileDirectoryDto
        {
            Id = dir.Id,
            Name = dir.Name,
            ParentId = dir.ParentId,
            Path = dir.Path,
            SortOrder = dir.SortOrder,
            Description = dir.Description,
            FileCount = dir.Files?.Count(f => !f.IsDeleted) ?? 0,
            Children = dir.Children?.Where(c => !c.IsDeleted).Select(MapToTree).ToList() ?? new(),
            CreateTime = dir.CreateTime,
            Creator = dir.Creator,
            UpdateTime = dir.UpdateTime
        };
    }
}
