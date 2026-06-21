using RiverLi.DDD.Core.Domain.Repositories;
using riverli.blog.services.file.Domain.Entities;

namespace riverli.blog.services.file.Application.Interfaces;

/// <summary>
/// 文件仓储接口
/// </summary>
public interface IFileRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task<FileItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FileItem?> GetByStoredNameAsync(string storedName, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<FileItem> Items, int TotalCount)> GetPagedListAsync(
        Guid? directoryId = null, string? keyword = null, string? contentType = null,
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<FileItem>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<List<FileItem>> GetByDirectoryIdRecursiveAsync(Guid directoryId, CancellationToken cancellationToken = default);

    Task<FileItem> AddAsync(FileItem entity, CancellationToken cancellationToken = default);
    Task<FileItem> UpdateAsync(FileItem entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(FileItem entity, CancellationToken cancellationToken = default);
}
