using IdentityService.Domain.Entities;

namespace IdentityService.WebApi.Controllers.UserAdmin
{
    public record UserDTO(string Id, string UserName, string PhoneNumber, DateTime CreateTime)
    {
        public static UserDTO Create(User u)
        {
            return new UserDTO(u.Id, u.UserName, u.PhoneNumber, u.CreateTime);
        }
    }


}
