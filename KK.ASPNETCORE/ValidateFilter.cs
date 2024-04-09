
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KK.ASPNETCORE
{
    public class ValidateFilter : IAsyncActionFilter
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ValidateFilter(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private ValidateAttribute? GetValidateAttr(ActionDescriptor desc)
        {
            var controllerActionDesc = desc as ControllerActionDescriptor;
            if (controllerActionDesc == null)
            {
                return null;
            }
            var attr = controllerActionDesc.ControllerTypeInfo.GetCustomAttribute<ValidateAttribute>();
            if (attr != null)
            {
                return attr;
            }
            return controllerActionDesc.MethodInfo.GetCustomAttribute<ValidateAttribute>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取Valiate特性
            var attr = GetValidateAttr(context.ActionDescriptor);
            if (attr == null)
            {
                await next();
                return;
            }
            Type validatorType = attr.ValidatorType;
            IValidator validator = ((IValidator)httpContextAccessor.HttpContext.RequestServices.GetRequiredService(validatorType));
            if (validator == null)
            {
                throw new ApplicationException($"there is no such validator type={validatorType}");
            }
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                if (validator.CanValidateInstancesOfType(parameter.ParameterType))
                {
                    var validationContext =
                         new ValidationContext<object>(context.ActionArguments[parameter.Name]);
                    var validateRes = validator.Validate(validationContext);
                    if (!validateRes.IsValid)
                    {
                        context.Result = new ContentResult() { StatusCode = 400, Content = validateRes.SumErrors() };
                        return;
                    }
                }
            }
            await next();
        }
    }
}
