using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs
{
    public class AlbumConfig : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.ToTable("T_Albums");
            builder.HasKey(a => a.Id).IsClustered(false);
            builder.HasIndex(a => new { a.IsDeleted, a.CategoryId });
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
