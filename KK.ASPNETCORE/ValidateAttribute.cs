
using FluentValidation;

namespace KK.ASPNETCORE
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ValidateAttribute : Attribute
    {
        public Type ValidatorType { get; init; }

        public ValidateAttribute(Type validatorType)
        {
            if (!validatorType.IsAssignableTo(typeof(IValidator)))
            {
                throw new ArgumentException($"the type={validatorType} is not assignable to IValidator");
            }
            this.ValidatorType = validatorType;
        }
    }
}
