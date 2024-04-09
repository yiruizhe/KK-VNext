using IdentityService.Domain;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure;

public class IdRepository : IIdRepository
{
    private readonly UserManager<User> userManager;
    private readonly RoleManager<Role> roleManager;
    private ILogger<IdRepository> logger;

    public IdRepository(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<IdRepository> logger)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.logger = logger;
    }

    public async Task<IdentityResult> AccessFailedAsync(User user)
    {
        return await userManager.AccessFailedAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        User user = await userManager.FindByIdAsync(userId);
        return await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }

    public async Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockOutOnFailure)
    {
        if (await userManager.IsLockedOutAsync(user))
        {
            return SignInResult.LockedOut;
        }
        bool checkRes = await userManager.CheckPasswordAsync(user, password);
        if (checkRes)
        {
            return SignInResult.Success;
        }
        else
        {
            if (lockOutOnFailure)
            {
                var r = await AccessFailedAsync(user);
                if (!r.Succeeded)
                {
                    throw new ApplicationException("AccessFailed failed");
                }
            }
            return SignInResult.Failed;
        }
    }

    public Task<IdentityResult> CreateAsync(User user, string password) => userManager.CreateAsync(user, password);

    public Task<User?> FindByIdAsync(string userId) => userManager.FindByIdAsync(userId);

    public Task<User?> FindByPhoneNumAsync(string phoneNum) => userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNum));

    public Task<User?> FindByUserNameAsync(string username) => userManager.FindByNameAsync(username);

    public Task<string> GenerageChangePhoneNumberTokenAsync(User user, string phoneNumber)
    {
        return userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
    }

    public async Task<SignInResult> ChangePhoneNumAsync(string userId, string phoneNum, string token)
    {
        User user = await userManager.FindByIdAsync(userId);
        var result = await userManager.ChangePhoneNumberAsync(user, phoneNum, token);
        if (!result.Succeeded)
        {
            await userManager.AccessFailedAsync(user);
            string errorMsg = string.Join('\n', result.Errors.Select(e => new { Code = e.Code, Desc = e.Description }));
            logger.LogWarning($"{phoneNum}ChangePhoneNumber Failed ，Error Message:{errorMsg}");
            return SignInResult.Failed;
        }
        else
        {
            await ConfirmPhoneNumber(user.Id);
            return SignInResult.Success;
        }

    }

    public async Task ConfirmPhoneNumber(string id)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (user == null)
        {
            throw new ArgumentException($"找不到用户,id={id}");
        }
        user.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(user);
    }

    public Task<IList<string>> GetRolesAsync(User user) => userManager.GetRolesAsync(user);

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            Role role = new Role() { Name = roleName };
            var res = await roleManager.CreateAsync(role);
            if (!res.Succeeded)
            {
                return res;
            }
        }
        return await userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<(IdentityResult result, User? user, string? password)> AddAdminUserAsync(string userName, string phoneNumber)
    {
        if (await FindByUserNameAsync(userName) != null)
        {
            return (ErrorResult($"用户名已被注册:{userName}"), null, null);
        }
        if (await FindByPhoneNumAsync(phoneNumber) != null)
        {
            return (ErrorResult($"手机号已注册:{phoneNumber}"), null, null);
        }
        var user = new User(userName);
        user.PhoneNumber = phoneNumber;
        user.PhoneNumberConfirmed = true;
        var result = await CreateAsync(user, "111111");
        if (!result.Succeeded)
        {
            return (result, null, null);
        }
        var result2 = await AddToRoleAsync(user, "Admin");
        if (!result2.Succeeded)
        {
            return (result, null, null);
        }
        return (IdentityResult.Success, user, "111111");
    }

    private IdentityResult ErrorResult(string result)
    {
        var error = new IdentityError() { Description = result };
        return IdentityResult.Failed(error);
    }

    public async Task<IdentityResult> RemoveUserAsync(string id)
    {
        User user = await userManager.FindByIdAsync(id);
        user.SoftDelete();
        return await userManager.UpdateAsync(user);
    }

    public async Task<(IdentityResult res, User? user, string? password)> ResetPasswordAsync(string id)
    {
        User? user = await FindByIdAsync(id);
        if (user == null)
        {
            return (IdentityResult.Failed(new IdentityError() { Description = "找不到用户" }), null, null);

        }
        string password = "111111";
        string token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
        {
            return (result, null, null);
        }
        return (result, user, password);
    }
}
