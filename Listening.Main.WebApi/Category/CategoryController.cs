using KK.ASPNETCORE;
using Listening.Domain;
using Listening.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Main.WebApi.Category;

[Route("[controller]/[action]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IDistributeCacheHelper cacheHelper;
    private readonly IListeningRepository repository;

    public CategoryController(IDistributeCacheHelper cacheHelper, IListeningRepository repository)
    {
        this.cacheHelper = cacheHelper;
        this.repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<CategoryVM[]>> FindAll()
    {
        string cacheKey = "Category_All";
        Task<Domain.Entities.Category[]> FindData()
        {
            return repository.GetCategoriesAsync();
        }
        var task = cacheHelper.GetOrCreateAsync(cacheKey, async (e) => CategoryVM.Create(await FindData()));
        return await task;
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<CategoryVM?>> FindOne(string categoryId)
    {
        string cacheKey = $"Category_{categoryId}";
        var res = await cacheHelper.GetOrCreateAsync(cacheKey, async (e) =>
            CategoryVM.Create(await repository.FindCategoryByIdAsync(categoryId))
        ) ;

        if (res == null)
        {
            return NotFound($"没有id={categoryId}的Category");
        }
        return Ok(res);
    }
}
