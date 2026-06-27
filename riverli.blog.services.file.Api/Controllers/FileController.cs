using MediatR;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Infrastructure.Shared.Auth;
using RiverLi.DDD.Core.Application.Common.Models;
using riverli.blog.services.file.Application.Constants;
using riverli.blog.services.file.Application.DTOs;
using riverli.blog.services.file.Application.Features.Files.Commands;
using riverli.blog.services.file.Application.Features.Files.Queries;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;
using RiverLi.DDD.Core.Application.Common.Interfaces;

namespace riverli.blog.services.file.Api.Controllers;

/// <summary>
/// 文件管理接口
/// </summary>
[ApiController]
[Route("api/file/[controller]")]
public class FileController : BaseApiController
{
    /// <summary>
    /// 上传图片（返回 URL，兼容旧 Blog 媒体接口）
    /// </summary>
    [HttpPost("upload-image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResult.FailResult("请选择要上传的文件", 400));

        var storageService = HttpContext.RequestServices.GetRequiredService<IStorageService>();
        var fileRepository = HttpContext.RequestServices.GetRequiredService<IFileRepository>();
        var currentUser = HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

        await using var stream = file.OpenReadStream();
        var storageResult = await storageService.UploadAsync(
            file.FileName, file.ContentType, stream, FileConstants.ImageBucket);

        if (!storageResult.Success)
            return StatusCode(500, ApiResult.FailResult($"上传失败: {storageResult.ErrorMessage}", 500));

        var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        var fileItem = new FileItem
        {
            FileName = file.FileName,
            StoredName = storageResult.StoredName,
            Extension = extension,
            ContentType = file.ContentType,
            FileSize = storageResult.FileSize,
            Bucket = FileConstants.ImageBucket,
            StoragePath = storageResult.StoragePath,
            Url = storageResult.Url,
            ThumbnailUrl = storageResult.ThumbnailUrl,
            Width = storageResult.Width,
            Height = storageResult.Height,
            IsPublic = true,
            Creator = currentUser.Id
        };

        await fileRepository.AddAsync(fileItem);
        await fileRepository.UnitOfWork.SaveEntitiesAsync();

        return Ok(storageResult.Url);
    }

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
