using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain;

public interface IIdRepository
{
    Task<SignInResult> CheckForSignInAsync(User user, string password, bool lockOutOnFailure);
    Task<User?> FindByPhoneNumAsync(string phoneNum);// 通过手机查询用户
    Task<User?> FindByUserNameAsync(string username);// 通过用户名查找用户
    Task<IList<string>> GetRolesAsync(User user); // 查询该用户所有角色
    Task<IdentityResult> CreateAsync(User user, string password);//创建用户
    Task<IdentityResult> AccessFailedAsync(User user);//  记录一次登录失败
    Task<User?> FindByIdAsync(string userId);
    /// <summary>
    /// 根据原有密码修改密码
    /// </summary>
    Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    /// <summary>
    /// 生成修改手机号的验证码
    /// </summary>
    Task<string> GenerageChangePhoneNumberTokenAsync(User user, string phoneNumber);
    /// <summary>
    /// 检查token，然后修改手机号
    /// </summary>
    Task<SignInResult> ChangePhoneNumAsync(string userId, string phoneNum, string token);
    /// <summary>
    /// 确认手机号
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task ConfirmPhoneNumber(string id);
    /// <summary>
    /// 给用户添加一个角色
    /// </summary>
    /// <param name="user"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    /// <summary>
    /// 添加一个管理员用户
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<(IdentityResult result, User? user, string? password)> AddAdminUserAsync(string userName, string phoneNumber);
    /// <summary>
    /// 删除管理员用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IdentityResult> RemoveUserAsync(string id);
    /// <summary>
    /// 重置密码，并把重置后的密码发送给用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<(IdentityResult res, User? user, string? password)> ResetPasswordAsync(string id);
}
