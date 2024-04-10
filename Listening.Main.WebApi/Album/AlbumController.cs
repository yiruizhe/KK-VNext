using KK.ASPNETCORE;
using Listening.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Main.WebApi.Album;

[Route("[controller]/[action]")]
[ApiController]
public class AlbumController : ControllerBase
{
    private readonly IDistributeCacheHelper cacheHelper;
    private readonly IListeningRepository repository;

    public AlbumController(IDistributeCacheHelper cacheHelper, IListeningRepository repository)
    {
        this.cacheHelper = cacheHelper;
        this.repository = repository;
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<AlbumVM[]?>> FindAllByCategoryId(string categoryId)
    {
        string cacheKey = $"Album_All_{categoryId}";
        Task<Domain.Entities.Album[]> FindData()
        {
            return repository.GetAlbumsByCategoryIdAsync(categoryId);
        }
        return await cacheHelper.GetOrCreateAsync(cacheKey, async (e) =>
        {
            var res = (await FindData()).Where(a=>a.IsVisible).ToArray();
            return AlbumVM.Create(res);
        });
    }

    [HttpGet("{albumId}")]
    public async Task<ActionResult<AlbumVM?>> FindOne(string albumId)
    {
        string cacheKey = $"Album_{albumId}";
        var res = await cacheHelper.GetOrCreateAsync(cacheKey,
            async (e) =>
            {
                var album = await repository.FindAlbumByIdAsync(albumId);
                if(album == null)
                {
                    return null;
                }
                return AlbumVM.Create(album.IsVisible ? album : null);
            });
        if (res == null)
        {
            return NotFound($"找不到Id={albumId}的专辑");
        }
        return res;
    }

}
