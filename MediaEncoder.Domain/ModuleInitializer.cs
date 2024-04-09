

using KK.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace MediaEncoder.Domain
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<MediaEncoderFactory>();
        }
    }
}
