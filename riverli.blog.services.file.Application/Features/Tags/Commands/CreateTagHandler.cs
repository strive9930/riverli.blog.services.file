using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;

namespace riverli.blog.services.file.Application.Features.Tags.Commands;

public class CreateTagHandler : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    private readonly IFileTagRepository _tagRepository;
    private readonly ILogger<CreateTagHandler> _logger;

    public CreateTagHandler(IFileTagRepository tagRepository, ILogger<CreateTagHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var existing = await _tagRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
            return Result<Guid>.FailResult("标签名称已存在", 400);

        var tag = new FileTag
        {
            Name = request.Name,
            Color = request.Color
        };

        await _tagRepository.AddAsync(tag, cancellationToken);
        await _tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("标签已创建: {TagName}, ID: {TagId}", tag.Name, tag.Id);
        return Result<Guid>.SuccessResult(tag.Id, "标签创建成功");
    }
}
