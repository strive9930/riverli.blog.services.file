using RiverLi.DDD.Core.Domain.Repositories;
using riverli.blog.services.file.Domain.Entities;

namespace riverli.blog.services.file.Application.Interfaces;

/// <summary>
/// 文件标签仓储接口
/// </summary>
public interface IFileTagRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task<FileTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FileTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<FileTag>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<FileTag> AddAsync(FileTag entity, CancellationToken cancellationToken = default);
    Task<FileTag> UpdateAsync(FileTag entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(FileTag entity, CancellationToken cancellationToken = default);
}
