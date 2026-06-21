using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Files.Queries;

public class GetFileHandler : IRequestHandler<GetFileQuery, Result<FileDto>>
{
    private readonly IFileRepository _fileRepository;

    public GetFileHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<Result<FileDto>> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null || file.IsDeleted)
            return Result<FileDto>.FailResult("文件不存在", 404);

        var dto = MapToDto(file);
        return Result<FileDto>.SuccessResult(dto);
    }

    private static FileDto MapToDto(Domain.Entities.FileItem file)
    {
        return new FileDto
        {
            Id = file.Id,
            FileName = file.FileName,
            StoredName = file.StoredName,
            Extension = file.Extension,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            Url = file.Url,
            ThumbnailUrl = file.ThumbnailUrl,
            DirectoryId = file.DirectoryId,
            DirectoryName = file.Directory?.Name,
            Width = file.Width,
            Height = file.Height,
            IsPublic = file.IsPublic,
            DownloadCount = file.DownloadCount,
            Description = file.Description,
            Tags = file.Tags.Select(t => new FileTagDto
            {
                Id = t.Id,
                Name = t.Name,
                Color = t.Color,
                CreateTime = t.CreateTime
            }).ToList(),
            CreateTime = file.CreateTime,
            Creator = file.Creator,
            UpdateTime = file.UpdateTime
        };
    }
}
