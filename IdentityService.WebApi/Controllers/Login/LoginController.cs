using IdentityService.Domain;
using IdentityService.Domain.Entities;
using KK.ASPNETCORE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebApi.Controllers.Login;
[Route("[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IIdRepository repository;
    private readonly IdDomainService domainService;

    public LoginController(IIdRepository repository, IdDomainService domainService)
    {
        this.repository = repository;
        this.domainService = domainService;
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> CreateWorld()
    {
        User user = new User("admin");
        var r = await repository.CreateAsync(user, "111111");
        string token = await repository.GenerageChangePhoneNumberTokenAsync(user, "19155103441");
        await repository.ChangePhoneNumAsync(user.Id, "19155103441", token);
        IdentityResult res = await repository.AddToRoleAsync(user, "User");
        IdentityResult res2 = await repository.AddToRoleAsync(user, "Admin");
        return Ok();
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetUserInfo()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        User? user = await repository.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return new UserResponse(user.Id, user.PhoneNumber, user.CreateTime);
    }

    /// <summary>
    /// 通过手机号密码登录
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [Validate(typeof(LoginByPhoneAndPwdValidator))]
    public async Task<ActionResult<string>> LoginByPhoneAndPwd(LoginByPhoneAndPwdRequest req)
    {
        //TODO 要通过行为验证码或图形验证码来防止暴力破解
        (var checkResult, string? token) = await domainService.LoginByPhoneAndPwdAsync(req.PhoneNumber, req.Password);
        if (checkResult.Succeeded)
        {
            return token;
        }
        else if (checkResult.IsLockedOut)
        {
            return StatusCode((int)HttpStatusCode.Locked, "此账号已锁定");
        }
        else
        {
            return BadRequest("用户名或密码错误");
        }
    }

    /// <summary>
    /// 根据用户名和密码登录
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [Validate(typeof(LoginByUserNameAndPwdValidator))]
    public async Task<ActionResult<string>> LoginByUserNameAndPwd(LoginByUserNameAndPwdRequest req)
    {
        (var result, string? token) = await domainService.LoginByUserNameAndPwdAsync(req.UserName, req.Password);
        if (result.Succeeded)
        {
            return token;
        }
        else if (result.IsLockedOut)
        {
            return StatusCode((int)HttpStatusCode.Locked, "此账号已锁定");
        }
        else
        {
            return BadRequest("用户名或密码错误");
        }
    }

    /// <summary>
    /// 修改密码,在没有忘记密码的情况之下
    /// </summary>
    [HttpPost]
    [Authorize]
    [Validate(typeof(ChangeMyPwdValidator))]
    public async Task<ActionResult> ChangeMyPassword(ChangeMyPwdRequest req)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await repository.ChangePasswordAsync(userId, req.OldPassword, req.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        // TODO 修改密码完成后应主动使用户退出，即主动使用户的token过期
        return Ok();
    }
}
