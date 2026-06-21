using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, Result>
{
    private readonly IFileRepository _fileRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<DeleteFileHandler> _logger;

    public DeleteFileHandler(IFileRepository fileRepository, IStorageService storageService, ILogger<DeleteFileHandler> logger)
    {
        _fileRepository = fileRepository;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null)
            return Result.FailResult("文件不存在", 404);

        // 删除存储文件
        await _storageService.DeleteAsync(file.StoragePath, cancellationToken);

        // 软删除数据库记录
        file.MarkAsDeleted();
        await _fileRepository.UpdateAsync(file, cancellationToken);
        await _fileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("文件已删除: {FileId} {FileName}", file.Id, file.FileName);
        return Result.SuccessResult("文件删除成功");
    }
}
