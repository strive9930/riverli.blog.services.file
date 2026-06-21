using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Features.Tags.Commands;
using riverli.blog.services.file.Application.Features.Tags.Queries;

namespace riverli.blog.services.file.Api.Controllers;

/// <summary>
/// 标签管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TagController : BaseApiController
{
    /// <summary>
    /// 获取所有标签
    /// </summary>
    [HttpGet]
    public async Task<ApiResult<List<FileTagDto>>> GetAll()
    {
        var result = await Mediator.Send(new GetTagsQuery());
        return result.Success
            ? ApiResult<List<FileTagDto>>.SuccessResult(result.Data!, result.Message)
            : ApiResult<List<FileTagDto>>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 创建标签
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<Guid>> Create([FromBody] CreateTagCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Success
            ? ApiResult<Guid>.SuccessResult(result.Data, result.Message)
            : ApiResult<Guid>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 删除标签
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteTagCommand { TagId = id });
        return result.Success
            ? ApiResult.SuccessResult(result.Message)
            : ApiResult.FailResult(result.Message, result.Code);
    }
}
