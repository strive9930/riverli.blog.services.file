using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Directories.Commands;

public class DeleteDirectoryHandler : IRequestHandler<DeleteDirectoryCommand, Result>
{
    private readonly IFileDirectoryRepository _directoryRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<DeleteDirectoryHandler> _logger;

    public DeleteDirectoryHandler(
        IFileDirectoryRepository directoryRepository,
        IFileRepository fileRepository,
        IStorageService storageService,
        ILogger<DeleteDirectoryHandler> logger)
    {
        _directoryRepository = directoryRepository;
        _fileRepository = fileRepository;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteDirectoryCommand request, CancellationToken cancellationToken)
    {
        var directory = await _directoryRepository.GetByIdAsync(request.DirectoryId, cancellationToken);
        if (directory is null)
            return Result.FailResult("目录不存在", 404);

        // 检查是否有子文件
        var files = await _fileRepository.GetByDirectoryIdRecursiveAsync(request.DirectoryId, cancellationToken);
        if (files.Count > 0)
            return Result.FailResult($"目录下有 {files.Count} 个文件，请先清空文件", 400);

        directory.MarkAsDeleted();
        await _directoryRepository.UpdateAsync(directory, cancellationToken);
        await _directoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("目录已删除: {DirectoryId} {DirectoryName}", directory.Id, directory.Name);
        return Result.SuccessResult("目录删除成功");
    }
}
