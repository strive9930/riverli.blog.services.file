using MediatR;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Features.Files.Commands;
using riverli.blog.services.file.Application.Features.Files.Queries;

namespace riverli.blog.services.file.Api.Controllers;

/// <summary>
/// 文件管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FileController : BaseApiController
{
    /// <summary>
    /// 上传文件
    /// </summary>
    [HttpPost("upload")]
    public async Task<ApiResult<Guid>> Upload([FromForm] UploadFileCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Success
            ? ApiResult<Guid>.SuccessResult(result.Data, result.Message)
            : ApiResult<Guid>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 获取文件详情
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ApiResult<FileDto>> Get(Guid id)
    {
        var result = await Mediator.Send(new GetFileQuery { FileId = id });
        return result.Success
            ? ApiResult<FileDto>.SuccessResult(result.Data!, result.Message)
            : ApiResult<FileDto>.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 获取文件列表（分页）
    /// </summary>
    [HttpGet("list")]
    public async Task<ApiResult<PagedResult<FileDto>>> GetList(
        [FromQuery] Guid? directoryId,
        [FromQuery] string? keyword,
        [FromQuery] string? contentType,
        [FromQuery] Guid? tagId,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFileListQuery
        {
            DirectoryId = directoryId,
            Keyword = keyword,
            ContentType = contentType,
            TagId = tagId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var result = await Mediator.Send(query);
        return ApiResult<PagedResult<FileDto>>.SuccessResult(result, "查询成功");
    }

    /// <summary>
    /// 更新文件信息
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ApiResult> Update(Guid id, [FromBody] UpdateFileInfoRequest request)
    {
        var command = new UpdateFileInfoCommand
        {
            FileId = id,
            FileName = request.FileName,
            Description = request.Description,
            IsPublic = request.IsPublic,
            DirectoryId = request.DirectoryId,
            TagIds = request.TagIds
        };
        var result = await Mediator.Send(command);
        return result.Success
            ? ApiResult.SuccessResult(result.Message)
            : ApiResult.FailResult(result.Message, result.Code);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteFileCommand { FileId = id });
        return result.Success
            ? ApiResult.SuccessResult(result.Message)
            : ApiResult.FailResult(result.Message, result.Code);
    }
}

/// <summary>
/// 更新文件信息请求体
/// </summary>
public class UpdateFileInfoRequest
{
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public Guid? DirectoryId { get; set; }
    public List<Guid>? TagIds { get; set; }
}
