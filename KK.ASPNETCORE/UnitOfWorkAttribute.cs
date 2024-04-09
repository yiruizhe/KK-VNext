using Microsoft.EntityFrameworkCore;

namespace KK.ASPNETCORE
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UnitOfWorkAttribute : Attribute
    {
        public Type[] DbContextTypes { get; init; }

        public UnitOfWorkAttribute(params Type[] dbContextTypes)
        {
            this.DbContextTypes = dbContextTypes;
            foreach (var item in dbContextTypes)
            {
                if (!item.IsAssignableTo(typeof(DbContext)))
                {
                    throw new ArgumentException($"{item} must inherit from dbcontext");
                }
            }
        }
    }
}
