using RiverLi.DDD.Core.Domain.Repositories;
using riverli.blog.services.file.Domain.Entities;

namespace riverli.blog.services.file.Application.Interfaces;

/// <summary>
/// 文件目录仓储接口
/// </summary>
public interface IFileDirectoryRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task<FileDirectory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<FileDirectory>> GetRootDirectoriesAsync(CancellationToken cancellationToken = default);
    Task<List<FileDirectory>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<List<FileDirectory>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task<FileDirectory?> GetByNameAndParentAsync(string name, Guid? parentId, CancellationToken cancellationToken = default);

    Task<FileDirectory> AddAsync(FileDirectory entity, CancellationToken cancellationToken = default);
    Task<FileDirectory> UpdateAsync(FileDirectory entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(FileDirectory entity, CancellationToken cancellationToken = default);
}
