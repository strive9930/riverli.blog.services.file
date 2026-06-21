using MediatR;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Tags.Commands;

public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Result>
{
    private readonly IFileTagRepository _tagRepository;
    private readonly ILogger<DeleteTagHandler> _logger;

    public DeleteTagHandler(IFileTagRepository tagRepository, ILogger<DeleteTagHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
            return Result.FailResult("标签不存在", 404);

        tag.MarkAsDeleted();
        await _tagRepository.UpdateAsync(tag, cancellationToken);
        await _tagRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("标签已删除: {TagId} {TagName}", tag.Id, tag.Name);
        return Result.SuccessResult("标签删除成功");
    }
}
