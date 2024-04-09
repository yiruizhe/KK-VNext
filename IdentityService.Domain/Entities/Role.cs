using KK.Commons;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class Role : IdentityRole<string>
{
    public Role()
    {
        Id = IdHelper.NextId;
    }
}
