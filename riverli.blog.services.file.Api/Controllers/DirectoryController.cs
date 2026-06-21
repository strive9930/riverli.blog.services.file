using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Features.Directories.Commands;
using riverli.blog.services.file.Application.Features.Directories.Queries;

namespace riverli.blog.services.file.Api.Controllers;

/// <summary>
/// 目录管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DirectoryController : BaseApiController
{
    /// <summary>
    /// 获取目录树
    /// </summary>
    [HttpGet("tree")]
    public async Task<ApiResult<List<FileDirectoryDto>>> GetTree()
    {
        var result = await Mediator.Send(new GetDirectoryTreeQuery());
        return result.Success
            ? ApiResult<List<FileDirectoryDto>>.SuccessResult(result.Data!, result.Message)
            : ApiResult<List<FileDirectoryDto>>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    [HttpPost]
    public async Task<ApiResult<Guid>> Create([FromBody] CreateDirectoryCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Success
            ? ApiResult<Guid>.SuccessResult(result.Data, result.Message)
            : ApiResult<Guid>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 删除目录
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteDirectoryCommand { DirectoryId = id });
        return result.Success
            ? ApiResult.SuccessResult(result.Message)
            : ApiResult.FailResult(result.Message, result.Code);
    }
}
