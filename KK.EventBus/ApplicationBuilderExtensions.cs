using Microsoft.AspNetCore.Builder;

namespace KK.EventBus;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEventBus(this IApplicationBuilder appBuilder)
    {
        object? eventBus = appBuilder.ApplicationServices.GetService(typeof(IEventBus));
        if (eventBus == null)
        {
            throw new ApplicationException("找不大EventBus实例");
        }

        return appBuilder;
    }
}