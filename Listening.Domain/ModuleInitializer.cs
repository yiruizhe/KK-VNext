using KK.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace Listening.Domain
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<ListeningDomainService>();
        }
    }
}
