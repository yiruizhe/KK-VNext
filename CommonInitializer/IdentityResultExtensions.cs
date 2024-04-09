using Microsoft.AspNetCore.Identity;

namespace CommonInitializer
{
    public static class IdentityResultExtensions
    {
        public static string SumErrors(this IdentityResult identityResult)
        {
            string[] errors = identityResult.Errors.Select(e => $" Code = {e.Code}, Desc = {e.Description}").ToArray();
            return string.Join('\n', errors);
        }
    }
}
