using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Interfaces;

namespace riverli.blog.services.file.Application.Features.Tags.Queries;

public class GetTagsHandler : IRequestHandler<GetTagsQuery, Result<List<FileTagDto>>>
{
    private readonly IFileTagRepository _tagRepository;

    public GetTagsHandler(IFileTagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<List<FileTagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        var dtos = tags.Select(t => new FileTagDto
        {
            Id = t.Id,
            Name = t.Name,
            Color = t.Color,
            FileCount = t.Files?.Count ?? 0,
            CreateTime = t.CreateTime
        }).ToList();

        return Result<List<FileTagDto>>.SuccessResult(dtos);
    }
}
