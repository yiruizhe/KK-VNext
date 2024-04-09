using CommonInitializer;
using FileService.Infrasructure;
using FileService.Infrasructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FileService.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.ConfigureExtraServices(new InitializerOptions
        {
            LogFilePath = "logs/FSService.log",
            EventBusQueueName = "FileService.WebApi"
        }, Assembly.Load("FileService.Domain"), Assembly.Load("FileService.Infrasructure"),
        Assembly.GetExecutingAssembly());

        // 添加数据库上下文
        builder.Services.AddDbContext<FSDbContext>(opt =>
        {
            string connStr = builder.Configuration.GetConnectionString("Default");
            opt.UseSqlServer(connStr);
        });
        // 添加文件服务配置项
        builder.Services.Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));
        // TODO 云存储配置

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
        }

        app.UseStaticFiles();

        app.UseKKDefault();

        app.MapControllers();

        app.Run();
    }
}
