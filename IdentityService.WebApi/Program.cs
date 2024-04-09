using CommonInitializer;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentityService.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.ConfigureExtraServices(new InitializerOptions
        {
            LogFilePath = "logs/IdentityService.log",
            EventBusQueueName = "IdentityService.WebApi"
        }, Assembly.GetExecutingAssembly(), Assembly.Load("IdentityService.Domain"), Assembly.Load("IdentityService.Infrastructure")
        );

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
            });

        // 添加上下文
        builder.Services.AddDbContext<IdDbContext>(opt =>
        {
            string connStr = builder.Configuration.GetConnectionString("Default");
            opt.UseSqlServer(connStr);
        });

        // 添加标识框架
        builder.Services.AddDataProtection();
        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequiredLength = 6;
        });
        var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), builder.Services);
        idBuilder.AddEntityFrameworkStores<IdDbContext>()
            .AddDefaultTokenProviders()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<UserManager<User>>();
        // 添加标识框架结束

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
        }

        app.UseKKDefault();
        app.MapControllers();
        app.Run();
    }
}
