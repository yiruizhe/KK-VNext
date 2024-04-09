using MediaEncoder.Domain;
using MediaEncoder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoder.Infrustructure;

public class MediaEncoderRepository : IMediaEncoderRepository
{
    private readonly MEDbcontext ctx;

    public MediaEncoderRepository(MEDbcontext ctx)
    {
        this.ctx = ctx;
    }

    public Task<EncodingItem[]> FindAsync(ItemStatus status)
    {
        return ctx.EncodingItems.Where(e => e.Status == status).ToArrayAsync();
    }

    public Task<EncodingItem?> FindCompletedOneAsync(long fileSize, string fileHash)
    {
        return ctx.EncodingItems.FirstOrDefaultAsync(
                    e => e.FileSizeInBytes == fileSize
                    && e.FileSHA256Hash == fileHash
                    && e.Status == ItemStatus.Completed);
    }
}
