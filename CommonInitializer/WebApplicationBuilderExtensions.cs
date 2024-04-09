using FluentValidation.AspNetCore;
using KK.ASPNETCORE;
using KK.Commons;
using KK.EventBus;
using KK.Infrastructure;
using KK.JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data.SqlClient;
using System.Reflection;

namespace CommonInitializer;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// 从数据库中加载项目配置
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureDbConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureAppConfiguration((hostCtx, configBuilder) =>
        {
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            configBuilder.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromMinutes(5));
        });
    }

    /// <summary>
    /// 配置依赖注入容器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="initOptions"></param>
    public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions
        , params Assembly[] assemblies)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        services.RunModuleInitializers(assemblies.ToList());
        JwtOptions jwtOpt = configuration.GetSection("Jwt").Get<JwtOptions>();
        services.AddJwtAuthentication(jwtOpt);
        services.AddAuthorization();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // 启用Swagger中的Authrize按钮
        services.Configure<SwaggerGenOptions>(opt =>
        {
            opt.AddAuthenticationHeader();
        });

        // 加入数据校验器
        services.AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblies(assemblies);
            fv.AutomaticValidationEnabled = false;
        });
        // 加入领域事件服务
        services.AddMediaR(assemblies);
        //注册过滤器
        services.Configure<MvcOptions>(opt =>
        {
            opt.Filters.Add<UnitOfWorkFilter>();
        });
        //添加跨域配置
        services.AddCors(opt =>
        {
            var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>();
            string[] urls = corsOpt.Origins;
            opt.AddDefaultPolicy(builder =>
            builder.WithOrigins(urls)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
        });
        //添加Serilog作为日志
        services.AddLogging(builder =>
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(initOptions.LogFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.AddSerilog();
        });

        //缓存配置
        string redisConnStr = configuration.GetValue<string>("Redis:ConnStr");
        var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
        services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);

        services.Configure<ForwardedHeadersOptions>(opt =>
        {
            opt.ForwardedHeaders = ForwardedHeaders.All;
        });
        // 集成事件配置
        services.Configure<IntegrationEventRabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddEventBus(initOptions.EventBusQueueName, assemblies);
    }

}
