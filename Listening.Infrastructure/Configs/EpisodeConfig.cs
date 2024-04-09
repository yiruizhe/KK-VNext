using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs
{
    public class EpisodeConfig : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.ToTable("T_Episodes");
            builder.HasKey(e => e.Id).IsClustered(false);
            builder.HasKey(e => new { e.IsDeleted, e.AlbumId });

            builder.Property(e => e.AudioUrl).HasMaxLength(1000).IsUnicode().IsRequired();
            builder.Property(e => e.SubTitle).HasMaxLength(int.MaxValue).IsUnicode().IsRequired();
            builder.Property(e => e.SubtitleType).HasMaxLength(10).IsUnicode(false).IsRequired();
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
