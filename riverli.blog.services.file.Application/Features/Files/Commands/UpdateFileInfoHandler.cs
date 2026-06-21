using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Infrastructure.Shared.Auth;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Files.Commands;

public class UpdateFileInfoHandler : IRequestHandler<UpdateFileInfoCommand, Result>
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileTagRepository _tagRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateFileInfoHandler> _logger;

    public UpdateFileInfoHandler(
        IFileRepository fileRepository,
        IFileTagRepository tagRepository,
        ICurrentUser currentUser,
        ILogger<UpdateFileInfoHandler> logger)
    {
        _fileRepository = fileRepository;
        _tagRepository = tagRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateFileInfoCommand request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null)
            return Result.FailResult("文件不存在", 404);

        file.UpdateInfo(request.FileName ?? file.FileName, request.Description, request.IsPublic);

        if (request.DirectoryId.HasValue)
            file.MoveTo(request.DirectoryId);

        // 更新标签
        if (request.TagIds is not null)
        {
            file.Tags.Clear();
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId, cancellationToken);
                if (tag is not null)
                    file.AddTag(tag);
            }
        }

        file.Updator = _currentUser.Id;

        await _fileRepository.UpdateAsync(file, cancellationToken);
        await _fileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("文件信息已更新: {FileId}", file.Id);
        return Result.SuccessResult("文件信息更新成功");
    }
}
