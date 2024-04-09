using CommonInitializer;
using MediaEncoder.Infrustructure;
using MediaEncoer.WebApi.BgServices;
using MediaEncoer.WebApi.Options;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MediaEncoer.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureExtraServices(new InitializerOptions()
            {
                LogFilePath = "logs/MediaEncoder.log",
                EventBusQueueName = "MediaEncoder.WebApi"
            }, Assembly.Load("MediaEncoder.Domain"), Assembly.Load("MediaEncoder.Infrustructure")
            , Assembly.GetExecutingAssembly());

            builder.Services.AddDbContext<MEDbcontext>(opt =>
            {
                string connStr = builder.Configuration.GetConnectionString("Default");
                opt.UseSqlServer(connStr);
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "MediaEncoder.WebAPI", Version = "v1" });
            });
            builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileService"));
            builder.Services.AddHostedService<EncodingBgService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediaEncoder.WebAPI v1"));
            }

            app.UseKKDefault();


            app.MapControllers();

            app.Run();
        }
    }
}
