using KK.Commons;
using MediaEncoder.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace MediaEncoder.Infrustructure
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<IMediaEncoderRepository, MediaEncoderRepository>();
            services.AddScoped<IMediaEncoder, ToM4AEncoer>();
        }
    }
}
