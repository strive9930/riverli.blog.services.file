using Microsoft.EntityFrameworkCore;
using riverli.blog.services.file.Application.Interfaces;
using riverli.blog.services.file.Domain.Entities;
using riverli.blog.services.file.Infrastructure.Data;
using RiverLi.DDD.Core.Domain.Repositories;

namespace riverli.blog.services.file.Infrastructure.Repositories;

/// <summary>
/// 文件仓储实现
/// </summary>
public class FileRepository : IFileRepository
{
    private readonly FileDbContext _context;

    public FileRepository(FileDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<FileItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Files.FindAsync([id], cancellationToken);

    public async Task<FileItem?> GetByStoredNameAsync(string storedName, CancellationToken cancellationToken = default)
        => await _context.Files.FirstOrDefaultAsync(f => f.StoredName == storedName, cancellationToken);

    public async Task<(IReadOnlyList<FileItem> Items, int TotalCount)> GetPagedListAsync(
        Guid? directoryId = null,
        string? keyword = null,
        string? contentType = null,
        int pageIndex = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Files
            .Include(f => f.Directory)
            .Include(f => f.Tags)
            .AsQueryable();

        if (directoryId.HasValue)
            query = query.Where(f => f.DirectoryId == directoryId.Value);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(f => f.FileName.Contains(keyword) || (f.Description != null && f.Description.Contains(keyword)));
        if (!string.IsNullOrWhiteSpace(contentType))
            query = query.Where(f => f.ContentType == contentType);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(f => f.CreateTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<FileItem>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default)
        => await _context.Files
            .Include(f => f.Directory)
            .Include(f => f.Tags)
            .Where(f => f.Tags.Any(t => t.Id == tagId))
            .OrderByDescending(f => f.CreateTime)
            .ToListAsync(cancellationToken);

    public async Task<List<FileItem>> GetByDirectoryIdRecursiveAsync(Guid directoryId, CancellationToken cancellationToken = default)
    {
        var childIds = await _context.Directories
            .Where(d => d.ParentId == directoryId)
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        var allIds = new List<Guid> { directoryId };
        allIds.AddRange(childIds);

        return await _context.Files
            .Where(f => f.DirectoryId.HasValue && allIds.Contains(f.DirectoryId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task<FileItem> AddAsync(FileItem entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Files.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public Task<FileItem> UpdateAsync(FileItem entity, CancellationToken cancellationToken = default)
    {
        _context.Files.Update(entity);
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(FileItem entity, CancellationToken cancellationToken = default)
    {
        _context.Files.Remove(entity);
        return Task.CompletedTask;
    }
}
