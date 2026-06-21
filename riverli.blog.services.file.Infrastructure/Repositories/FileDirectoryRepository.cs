using Microsoft.EntityFrameworkCore;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;
using riverli.blog.services.file.Infrastructure.Data;

namespace riverli.blog.services.file.Infrastructure.Repositories;

/// <summary>
/// 文件目录仓储实现
/// </summary>
public class FileDirectoryRepository : IFileDirectoryRepository
{
    private readonly FileDbContext _context;

    public FileDirectoryRepository(FileDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<FileDirectory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Directories.FindAsync([id], cancellationToken);

    public async Task<List<FileDirectory>> GetRootDirectoriesAsync(CancellationToken cancellationToken = default)
        => await _context.Directories
            .Where(d => d.ParentId == null)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<List<FileDirectory>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
        => await _context.Directories
            .Where(d => d.ParentId == parentId)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<List<FileDirectory>> GetTreeAsync(CancellationToken cancellationToken = default)
        => await _context.Directories
            .Include(d => d.Children)
            .Include(d => d.Files)
            .Where(d => d.ParentId == null)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<FileDirectory?> GetByNameAndParentAsync(string name, Guid? parentId, CancellationToken cancellationToken = default)
        => await _context.Directories
            .FirstOrDefaultAsync(d => d.Name == name && d.ParentId == parentId, cancellationToken);

    public async Task<FileDirectory> AddAsync(FileDirectory entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Directories.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public Task<FileDirectory> UpdateAsync(FileDirectory entity, CancellationToken cancellationToken = default)
    {
        _context.Directories.Update(entity);
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(FileDirectory entity, CancellationToken cancellationToken = default)
    {
        _context.Directories.Remove(entity);
        return Task.CompletedTask;
    }
}
