using IdentityService.Domain;
using KK.Commons;
using KK.JWT;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<IdDomainService>();
            services.AddScoped<IIdRepository, IdRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISmsSender, MockSmsSender>();
            services.AddHttpContextAccessor();
        }
    }
}
