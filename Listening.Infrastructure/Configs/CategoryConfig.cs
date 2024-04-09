using Listening.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listening.Infrastructure.Configs
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("T_Categories");
            builder.HasKey(c => c.Id).IsClustered(false);
            builder.Property(c => c.CoverUrl).IsRequired(false).HasMaxLength(500).IsUnicode();
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
