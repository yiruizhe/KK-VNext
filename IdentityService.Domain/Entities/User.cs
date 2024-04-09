using KK.Commons;
using KK.DomainCommons;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public class User : IdentityUser<string>, IHasCreateTime, IHasDeleteTime, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public DateTime? DeleteTime { get; private set; }

    public DateTime CreateTime { get; init; }

    public User(string userName) : base(userName)
    {
        Id = IdHelper.NextId;
        CreateTime = DateTime.Now;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeleteTime = DateTime.Now;
    }
}
