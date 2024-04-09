using FileService.Domain;
using FileService.Infrasructure.Services;
using KK.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrasructure
{
    public class ModuleInitializer : IModuleInitialier
    {
        public void Initilize(IServiceCollection services)
        {
            services.AddScoped<FSDomainService>();
            services.AddScoped<IFSRepository, FSRepository>();
            services.AddScoped<IStorageClient, SMBStorageClient>();
            services.AddScoped<IStorageClient, MockCloudStorageClient>();
            //services.AddScoped<IStorageClient, UpCloudStorageClient>();

            services.AddHttpContextAccessor();
            services.AddHttpClient();
        }
    }
}
