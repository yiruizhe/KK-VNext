using MediaEncoder.Domain.Entities;

namespace MediaEncoder.Domain
{
    public interface IMediaEncoderRepository
    {
        Task<EncodingItem?> FindCompletedOneAsync(long fileSize, string fileHash);

        Task<EncodingItem[]> FindAsync(ItemStatus status);
    }
}
