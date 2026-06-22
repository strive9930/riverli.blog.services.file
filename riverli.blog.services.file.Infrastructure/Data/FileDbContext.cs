using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Infrastructure.Shared.Auth;
using RiverLi.Blog.Infrastructure.Shared.Data;
using riverli.blog.services.file.Domain.Entities;
using RiverLi.DDD.Core.Application.Common.Interfaces;

namespace riverli.blog.services.file.Infrastructure.Data;

/// <summary>
/// 文件服务数据库上下文
/// </summary>
public class FileDbContext : RiverDbContext
{
    public FileDbContext(DbContextOptions options, IMediator mediator, ICurrentUser currentUser) : base(options, mediator, currentUser)
    {
    }

    public DbSet<FileItem> Files => Set<FileItem>();
    public DbSet<FileDirectory> Directories => Set<FileDirectory>();
    public DbSet<FileTag> Tags => Set<FileTag>();
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FileItem 配置
        modelBuilder.Entity<FileItem>(entity =>
        {
            entity.ToTable("FileItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StoredName).IsUnique();
            entity.HasIndex(e => e.DirectoryId);
            entity.HasIndex(e => e.ContentType);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.FileName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.StoredName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Extension).HasMaxLength(32);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Bucket).IsRequired().HasMaxLength(64);
            entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(1024);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1024);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Creator).HasMaxLength(64);
            entity.Property(e => e.Updator).HasMaxLength(64);

            entity.HasOne(e => e.Directory)
                .WithMany(d => d.Files)
                .HasForeignKey(e => e.DirectoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Tags)
                .WithMany(t => t.Files)
                .UsingEntity(j => j.ToTable("FileItemTags"));
        });

        // FileDirectory 配置
        modelBuilder.Entity<FileDirectory>(entity =>
        {
            entity.ToTable("FileDirectories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Path);
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Path).IsRequired().HasMaxLength(1024);
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.Creator).HasMaxLength(64);
            entity.Property(e => e.Updator).HasMaxLength(64);

            entity.HasOne(e => e.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FileTag 配置
        modelBuilder.Entity<FileTag>(entity =>
        {
            entity.ToTable("FileTags");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(16);
        });
    }
}
