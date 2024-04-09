using KK.Commons;
using Listening.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Listening.Infrastructure
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<IListeningRepository, ListeningRepository>();
        }
    }
}
