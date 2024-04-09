using KK.Infrastructure;
using MediaEncoder.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MediaEncoder.Infrustructure
{
    public class MEDbcontext : BaseDbContext
    {
        public DbSet<EncodingItem> EncodingItems { get; set; }

        public MEDbcontext(DbContextOptions options, IMediator mediator) : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
