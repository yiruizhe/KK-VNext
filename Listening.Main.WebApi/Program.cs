using CommonInitializer;
using KK.ASPNETCORE;
using Listening.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Listening.Main.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.ConfigureExtraServices(new InitializerOptions()
            {
                EventBusQueueName = "Listening.Main.WebApi",
                LogFilePath = "logs/listeningMain.log"
            }, Assembly.Load("Listening.Domain"), Assembly.GetExecutingAssembly(), Assembly.Load("Listening.Infrastructure"));
            builder.Services.AddDbContext<ListeningDbContext>(opt =>
            {
                string connStr = builder.Configuration.GetConnectionString("Default");
                opt.UseSqlServer(connStr);
            });
            builder.Services.AddStackExchangeRedisCache(opt =>
            {
                opt.InstanceName = "listeningMain_";
                opt.Configuration = builder.Configuration.GetValue<string>("Redis:ConnStr");
            });
            builder.Services.AddScoped<IDistributeCacheHelper, DistributedCacheHelper>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseKKDefault();

            app.MapControllers();

            app.Run();
        }
    }
}
