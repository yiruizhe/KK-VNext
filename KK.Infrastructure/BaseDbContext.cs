using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KK.Infrastructure;

public class BaseDbContext : DbContext
{
    private IMediator mediator;

    public BaseDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        this.mediator = mediator;
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotImplementedException("dn not call savechanges , call savechangesasync instead");
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        if (mediator != null)
        {
            await mediator.DispatchDomainEventsAsync(this);
        }
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
