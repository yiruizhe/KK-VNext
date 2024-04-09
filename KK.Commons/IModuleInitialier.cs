using Microsoft.Extensions.DependencyInjection;

namespace KK.Commons;

public interface IModuleInitialier
{
    public void Initilize(IServiceCollection services);
}
