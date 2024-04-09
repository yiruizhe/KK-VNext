using MediaEncoder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaEncoder.Infrustructure.Configs
{
    public class EncodingItemConfig : IEntityTypeConfiguration<EncodingItem>
    {
        public void Configure(EntityTypeBuilder<EncodingItem> builder)
        {
            builder.ToTable("T_EncodingItems");
            builder.Property(x => x.FileName).HasMaxLength(255);
            builder.Property(x => x.FileSHA256Hash).HasMaxLength(255).IsUnicode(false);
            builder.Property(x => x.OutputFormat).HasMaxLength(10).IsUnicode(false);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        }
    }
}
