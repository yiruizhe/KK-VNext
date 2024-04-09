using CommonInitializer;
using Listening.Admin.WebApi.Episodes;
using Listening.Admin.WebApi.Hubs;
using Listening.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Listening.Admin.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureExtraServices(new InitializerOptions()
            {
                LogFilePath = "logs/Listening_Admin.log",
                EventBusQueueName = "Listening.Admin"
            }, Assembly.Load("Listening.Domain"), Assembly.GetExecutingAssembly(), Assembly.Load("Listening.Infrastructure"));

            builder.Services.AddDbContext<ListeningDbContext>(opt =>
            {
                string connStr = builder.Configuration.GetConnectionString("Default");
                opt.UseSqlServer(connStr);
            });
            builder.Services.AddScoped<EncodingEpisodeCacheHelper>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Listening.Admin.WebAPI", Version = "v1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listening.Admin.WebAPI v1"));
            }

            app.UseKKDefault();

            app.MapHub<EpisodeEncodingStatusHub>("/Hubs/EpisodeEncodingStatusHub");

            app.MapControllers();

            app.Run();
        }
    }
}
