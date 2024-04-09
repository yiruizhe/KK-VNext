using KK.DomainCommons;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KK.Infrastructure
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediaR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddMediatR(assemblies.ToArray());
            return services;
        }

        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<IDomainEvents>().Where(e => e.Entity.GetDomainEvents().Any());
            var domainEvents = domainEntities.SelectMany(e => e.Entity.GetDomainEvents()).ToList();
            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvents());
            foreach (var eventItem in domainEvents)
            {
                await mediator.Publish(eventItem);
            }
        }
    }
}
