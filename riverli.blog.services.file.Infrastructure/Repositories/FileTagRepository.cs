using Microsoft.EntityFrameworkCore;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;
using riverli.blog.services.file.Infrastructure.Data;

namespace riverli.blog.services.file.Infrastructure.Repositories;

/// <summary>
/// 文件标签仓储实现
/// </summary>
public class FileTagRepository : IFileTagRepository
{
    private readonly FileDbContext _context;

    public FileTagRepository(FileDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<FileTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Tags.FindAsync([id], cancellationToken);

    public async Task<FileTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);

    public async Task<List<FileTag>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tags
            .Include(t => t.Files)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task<FileTag> AddAsync(FileTag entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Tags.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public Task<FileTag> UpdateAsync(FileTag entity, CancellationToken cancellationToken = default)
    {
        _context.Tags.Update(entity);
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(FileTag entity, CancellationToken cancellationToken = default)
    {
        _context.Tags.Remove(entity);
        return Task.CompletedTask;
    }
}
