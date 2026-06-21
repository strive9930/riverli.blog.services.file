using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Infrastructure.Shared.Auth;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;

namespace riverli.blog.services.file.Application.Features.Directories.Commands;

public class CreateDirectoryHandler : IRequestHandler<CreateDirectoryCommand, Result<Guid>>
{
    private readonly IFileDirectoryRepository _directoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<CreateDirectoryHandler> _logger;

    public CreateDirectoryHandler(
        IFileDirectoryRepository directoryRepository,
        ICurrentUser currentUser,
        ILogger<CreateDirectoryHandler> logger)
    {
        _directoryRepository = directoryRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateDirectoryCommand request, CancellationToken cancellationToken)
    {
        // 检查同级同名
        var existing = await _directoryRepository.GetByNameAndParentAsync(request.Name, request.ParentId, cancellationToken);
        if (existing is not null)
            return Result<Guid>.FailResult("同级目录下已存在同名目录", 400);

        // 获取父目录路径
        string? parentPath = null;
        if (request.ParentId.HasValue)
        {
            var parent = await _directoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
                return Result<Guid>.FailResult("父目录不存在", 404);
            parentPath = parent.Path;
        }

        var directory = new FileDirectory
        {
            Name = request.Name,
            ParentId = request.ParentId,
            Description = request.Description,
            Creator = _currentUser.Id
        };
        directory.RecalculatePath(parentPath);

        await _directoryRepository.AddAsync(directory, cancellationToken);
        await _directoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("目录已创建: {DirectoryName}, ID: {DirectoryId}", directory.Name, directory.Id);
        return Result<Guid>.SuccessResult(directory.Id, "目录创建成功");
    }
}
