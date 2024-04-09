using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrasructure.Configs
{
    public class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
    {
        public void Configure(EntityTypeBuilder<UploadedItem> builder)
        {
            builder.ToTable("T_FS_UploadedItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileSHA256Hash).IsUnicode(false).HasMaxLength(255);
            builder.Property(x => x.FileName).IsUnicode().HasMaxLength(255);
            builder.HasIndex(x => new { x.FileSizeInBytes, x.FileSHA256Hash });
        }
    }
}
