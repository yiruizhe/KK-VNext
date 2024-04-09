using IdentityService.Domain.Entities;
using KK.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityService.Domain;

public class IdDomainService
{
    private readonly IIdRepository idRepository;
    private readonly IOptions<JwtOptions> jwtOpt;
    private readonly ITokenService tokenService;

    public IdDomainService(IIdRepository idRepository, IOptions<JwtOptions> jwtOpt, ITokenService tokenService)
    {
        this.idRepository = idRepository;
        this.jwtOpt = jwtOpt;
        this.tokenService = tokenService;
    }


    /// <summary>
    /// 通过手机号和密码登录
    /// </summary>
    /// <returns></returns>
    public async Task<(SignInResult Result, string? Token)> LoginByPhoneAndPwdAsync(string phoneNum, string password)
    {
        SignInResult checkResult = await CheckPhoneNumAndPasswordAsync(phoneNum, password);
        if (checkResult == SignInResult.Success)
        {
            User? user = await idRepository.FindByPhoneNumAsync(phoneNum);
            string token = await BuildTokenAsync(user);
            return (checkResult, token);
        }
        else
        {
            return (checkResult, null);
        }
    }

    /// <summary>
    /// 通过用户名和密码登录
    /// </summary>
    /// <returns></returns>
    public async Task<(SignInResult Result, string? Token)> LoginByUserNameAndPwdAsync(string username, string password)
    {
        SignInResult checkResult = await CheckUserNameAndPasswordAsync(username, password);
        if (checkResult == SignInResult.Success)
        {
            User? user = await idRepository.FindByUserNameAsync(username);
            string token = await BuildTokenAsync(user);
            return (checkResult, token);
        }
        else
        {
            return (checkResult, null);
        }
    }

    private async Task<SignInResult> CheckUserNameAndPasswordAsync(string username, string password)
    {
        User? user = await idRepository.FindByUserNameAsync(username);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        return await idRepository.CheckForSignInAsync(user, password, true);
    }

    private async Task<SignInResult> CheckPhoneNumAndPasswordAsync(string phoneNum, string password)
    {
        User? user = await idRepository.FindByPhoneNumAsync(phoneNum);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        return await idRepository.CheckForSignInAsync(user, password, true);
    }

    private async Task<string> BuildTokenAsync(User user)
    {
        IList<string> roles = await idRepository.GetRolesAsync(user);
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return tokenService.BuildToken(claims, jwtOpt.Value);
    }
}
