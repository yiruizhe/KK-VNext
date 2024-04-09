using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrasructure;

public class FSRepository : IFSRepository
{
    private readonly FSDbContext ctx;

    public FSRepository(FSDbContext ctx)
    {
        this.ctx = ctx;
    }

    public Task<UploadedItem?> FindAsync(long fileSize, string fileSha256Hash)
    {
        return ctx.UploadedItems.FirstOrDefaultAsync(x => x.FileSizeInBytes == fileSize && x.FileSHA256Hash.Equals(fileSha256Hash));
    }
}
