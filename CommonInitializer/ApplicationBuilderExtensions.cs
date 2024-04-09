using KK.EventBus;
using Microsoft.AspNetCore.Builder;

namespace CommonInitializer;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseKKDefault(this IApplicationBuilder app)
    {
        app.UseEventBus();
        app.UseCors();
        app.UseForwardedHeaders();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
