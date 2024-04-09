using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Transactions;

namespace KK.ASPNETCORE;

public class UnitOfWorkFilter : IAsyncActionFilter
{

    private readonly ILogger<UnitOfWorkFilter> logger;

    public UnitOfWorkFilter(ILogger<UnitOfWorkFilter> logger)
    {
        this.logger = logger;
    }

    private UnitOfWorkAttribute? GetUoWAttr(ActionDescriptor actionDescriptor)
    {
        ControllerActionDescriptor? controllerAction = actionDescriptor as ControllerActionDescriptor;
        if (controllerAction == null)
        {
            return null;
        }
        var attr = controllerAction.ControllerTypeInfo.GetCustomAttribute<UnitOfWorkAttribute>();
        if (attr != null)
        {
            return attr;
        }
        else
        {
            return controllerAction.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
        }
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        UnitOfWorkAttribute? attr = GetUoWAttr(context.ActionDescriptor);
        if (attr == null)
        {
            await next();
            return;
        }
        using TransactionScope transaction = new(TransactionScopeAsyncFlowOption.Enabled);
        var sp = context.HttpContext.RequestServices;
        List<DbContext> instaces = new();
        foreach (var type in attr.DbContextTypes)
        {
            DbContext dbCtx = (DbContext)sp.GetRequiredService(type);
            instaces.Add(dbCtx);
        }
        var result = await next();
        if (result.Exception == null)
        {
            foreach (var item in instaces)
            {
                await item.SaveChangesAsync();
            }
            transaction.Complete();
        }
    }
}
