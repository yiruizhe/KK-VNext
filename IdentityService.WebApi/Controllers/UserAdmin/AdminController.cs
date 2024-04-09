using CommonInitializer;
using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.WebApi.Controllers.Events;
using KK.ASPNETCORE;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.WebApi.Controllers.UserAdmin;

[Route("[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IIdRepository repository;
    private readonly UserManager<User> userManager;
    private readonly IMediator mediator;

    public AdminController(IIdRepository repository, UserManager<User> userManager, IMediator mediator)
    {
        this.repository = repository;
        this.userManager = userManager;
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<UserDTO[]>> FindAllUsers()
    {
        return await userManager.Users.Select(u => UserDTO.Create(u)).ToArrayAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> FindById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return UserDTO.Create(user);
    }

    [HttpPost]
    [Validate(typeof(AddAdminUserValidator))]
    public async Task<ActionResult> AddAdminUser(AddAdminUserRequest req)
    {
        (IdentityResult result, User? user, string? password) = await repository.AddAdminUserAsync(req.UserName, req.PhoneNumber);
        if (!result.Succeeded)
        {
            return BadRequest(result.SumErrors());
        }
        // 发布创建用户的领域事件
        await mediator.Publish(new UserCreatedEvent(user.Id, user.UserName, password, user.PhoneNumber));
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAdminUser(string id)
    {
        await repository.RemoveUserAsync(id);
        return Ok();
    }


    [HttpPut("{id}")]
    [Validate(typeof(EditAdminUserValidator))]
    public async Task<ActionResult> UpdateAdminUser(string id, EditAdminUserRequest req)
    {
        var user = await repository.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("找不到用户");
        }
        user.PhoneNumber = req.PhoneNum;
        await userManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> ResetPassword(string id)
    {
        (IdentityResult res, User user, string password) = await repository.ResetPasswordAsync(id);
        if (!res.Succeeded)
        {
            return BadRequest(res.SumErrors());
        }
        // 把生成的密码发送给用户
        await mediator.Publish(new ResetPasswordEvent(user.Id, user.UserName, password, user.PhoneNumber));
        return Ok();
    }
}
