using CommonInitializer;
using FluentValidation;
using KK.ASPNETCORE;
using Listening.Admin.WebApi.Albums.Request;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Admin.WebApi.Albums;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWork(typeof(ListeningDbContext))]
public class AlbumController : ControllerBase
{
    private readonly IListeningRepository repository;
    private readonly ListeningDomainService domainService;
    private readonly ListeningDbContext ctx;

    private readonly IValidator<AlbumAddRequest> albumAddValidator;
    private readonly IValidator<AlbumUpdateRequest> albumUpdateValidator;
    private readonly IValidator<AlbumSortRequest> albumSortValidator;

    public AlbumController(IListeningRepository repository, ListeningDomainService domainService,
        ListeningDbContext ctx, IValidator<AlbumAddRequest> albumAddValidator,
        IValidator<AlbumUpdateRequest> albumUpdateValidator, IValidator<AlbumSortRequest> albumSortValidator)
    {
        this.repository = repository;
        this.domainService = domainService;
        this.ctx = ctx;
        this.albumAddValidator = albumAddValidator;
        this.albumUpdateValidator = albumUpdateValidator;
        this.albumSortValidator = albumSortValidator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Album>> FindById(string id)
    {
        var album = await repository.FindAlbumByIdAsync(id);
        if (album == null)
        {
            return NotFound("id不存在");
        }
        return album;
    }

    [HttpGet("{categoryId}")]
    public async Task<Album[]> FindByCategoryId(string categoryId)
    {
        return await repository.GetAlbumsByCategoryIdAsync(categoryId);
    }

    [HttpPost]
    public async Task<ActionResult<string>> Add(AlbumAddRequest req)
    {
        var res = albumAddValidator.Validate(req);
        if (!res.IsValid)
        {
            return BadRequest(res.SumErrors());
        }
        Album album = await domainService.AddAlbumAsync(req.Name, req.CategoryId);
        ctx.Albums.Add(album);
        return album.Id;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById(string id)
    {
        var album = await repository.FindAlbumByIdAsync(id);
        if (album == null)
        {
            return NotFound("id不存在");
        }
        album.SoftDelete();
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, AlbumUpdateRequest req)
    {
        var res = albumUpdateValidator.Validate(req);
        if (!res.IsValid)
        {
            return BadRequest(res.SumErrors());
        }
        var album = await repository.FindAlbumByIdAsync(id);
        if (album == null)
        {
            return NotFound("id不存在");
        }
        album.ChangeName(req.Name);
        return Ok();
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult> Sort(string categoryId, AlbumSortRequest req)
    {
        var res = albumSortValidator.Validate(req);
        if (!res.IsValid)
        {
            return BadRequest(res.SumErrors());
        }
        await domainService.SortAlbumsAsync(categoryId, req.SortedIds);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Hide(string id)
    {
        var album = await repository.FindAlbumByIdAsync(id);
        if (album == null)
        {
            return NotFound("id不存在");
        }
        album.Hide();
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Show(string id)
    {
        var album = await repository.FindAlbumByIdAsync(id);
        if (album == null)
        {
            return NotFound("id不存在");
        }
        album.Show();
        return Ok();
    }
}
