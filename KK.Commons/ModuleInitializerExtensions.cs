using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KK.Commons;

public static class ModuleInitializerExtensions
{
    /// <summary>
    /// 每个项目都有自己ModuleInitializer的实现类，在其中注册自己的服务，这样就不用都写在入口项目中
    /// </summary>
    /// <param name="service"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection RunModuleInitializers(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        foreach (var ass in assemblies)
        {
            IEnumerable<Type> moduleInitializers = ass.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IModuleInitialier)));
            foreach (var implType in moduleInitializers)
            {
                IModuleInitialier? instance = (IModuleInitialier?)Activator.CreateInstance(implType);
                if (instance == null)
                {
                    throw new ApplicationException($"can not create {implType}");
                }
                instance.Initilize(services);
            }
        }
        return services;
    }
}
