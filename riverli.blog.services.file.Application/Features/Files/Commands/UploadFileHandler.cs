using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Infrastructure.Shared.Auth;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Constants;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;
using RiverLi.DDD.Core.Application.Common.Interfaces;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

/// <summary>
/// 上传文件处理器
/// </summary>
public class UploadFileHandler : IRequestHandler<UploadFileCommand, Result<Guid>>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileTagRepository _tagRepository;
    private readonly IStorageService _storageService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UploadFileHandler> _logger;

    public UploadFileHandler(
        IFileRepository fileRepository,
        IFileTagRepository tagRepository,
        IStorageService storageService,
        ICurrentUser currentUser,
        ILogger<UploadFileHandler> logger)
    {
        _fileRepository = fileRepository;
        _tagRepository = tagRepository;
        _storageService = storageService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;

        // 校验文件大小
        if (file.Length > FileConstants.MaxFileSize)
            return Result<Guid>.FailResult($"文件大小不能超过 {FileConstants.MaxFileSize / 1024 / 1024}MB", 400);

        // 确定存储桶
        var bucket = request.Bucket ?? DetermineBucket(file.ContentType);

        // 上传到存储服务
        await using var stream = file.OpenReadStream();
        var storageResult = await _storageService.UploadAsync(
            file.FileName, file.ContentType, stream, bucket, cancellationToken);

        if (!storageResult.Success)
            return Result<Guid>.FailResult($"文件上传失败: {storageResult.ErrorMessage}", 500);

        // 创建文件实体
        var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        var fileItem = new FileItem
        {
            FileName = file.FileName,
            StoredName = storageResult.StoredName,
            Extension = extension,
            ContentType = file.ContentType,
            FileSize = storageResult.FileSize,
            Bucket = bucket,
            StoragePath = storageResult.StoragePath,
            Url = storageResult.Url,
            ThumbnailUrl = storageResult.ThumbnailUrl,
            DirectoryId = request.DirectoryId,
            Width = storageResult.Width,
            Height = storageResult.Height,
            IsPublic = request.IsPublic,
            Description = request.Description,
            Creator = _currentUser.Id
        };

        // 关联标签
        if (request.TagIds is { Count: > 0 })
        {
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId, cancellationToken);
                if (tag is not null)
                    fileItem.AddTag(tag);
            }
        }

        await _fileRepository.AddAsync(fileItem, cancellationToken);
        await _fileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("文件上传成功: {FileName}, 大小: {FileSize}, ID: {FileId}",
            file.FileName, storageResult.FileSize, fileItem.Id);

        return Result<Guid>.SuccessResult(fileItem.Id, "文件上传成功");
    }

    private static string DetermineBucket(string contentType)
    {
        if (FileConstants.ImageContentTypes.Contains(contentType))
            return FileConstants.ImageBucket;
        return FileConstants.DefaultBucket;
    }
}
