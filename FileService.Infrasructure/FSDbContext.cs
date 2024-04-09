using FileService.Domain.Entities;
using KK.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrasructure;

public class FSDbContext : BaseDbContext
{
    public DbSet<UploadedItem> UploadedItems { get; set; }

    public FSDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}
