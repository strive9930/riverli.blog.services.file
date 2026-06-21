using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Files.Queries;

public class GetFileListHandler : IRequestHandler<GetFileListQuery, PagedResult<FileDto>>
{
    private readonly IFileRepository _fileRepository;

    public GetFileListHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<PagedResult<FileDto>> Handle(GetFileListQuery request, CancellationToken cancellationToken)
    {
        List<Domain.Entities.FileItem> files;
        int totalCount;

        if (request.TagId.HasValue)
        {
            files = await _fileRepository.GetByTagIdAsync(request.TagId.Value, cancellationToken);
            totalCount = files.Count;
            files = files.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        }
        else
        {
            var result = await _fileRepository.GetPagedListAsync(
                request.DirectoryId, request.Keyword, request.ContentType,
                request.PageIndex, request.PageSize, cancellationToken);
            files = result.Items.ToList();
            totalCount = result.TotalCount;
        }

        var dtos = files.Select(f => new FileDto
        {
            Id = f.Id,
            FileName = f.FileName,
            StoredName = f.StoredName,
            Extension = f.Extension,
            ContentType = f.ContentType,
            FileSize = f.FileSize,
            Url = f.Url,
            ThumbnailUrl = f.ThumbnailUrl,
            DirectoryId = f.DirectoryId,
            DirectoryName = f.Directory?.Name,
            Width = f.Width,
            Height = f.Height,
            IsPublic = f.IsPublic,
            DownloadCount = f.DownloadCount,
            Description = f.Description,
            Tags = f.Tags.Select(t => new FileTagDto
            {
                Id = t.Id, Name = t.Name, Color = t.Color, CreateTime = t.CreateTime
            }).ToList(),
            CreateTime = f.CreateTime,
            Creator = f.Creator,
            UpdateTime = f.UpdateTime
        }).ToList();

        return new PagedResult<FileDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}
