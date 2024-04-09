using CommonInitializer;
using FluentValidation;
using KK.ASPNETCORE;
using Listening.Admin.WebApi.Categories.Request;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Listening.Admin.WebApi.Categories;

[Route("[controller]/[action]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWork(typeof(ListeningDbContext))]
public class CategoryController : ControllerBase
{
    private readonly IListeningRepository repository;
    private readonly ListeningDomainService domainService;
    private readonly ListeningDbContext ctx;
    // 数据校验器
    private readonly IValidator<CategoryAddRequest> cateAddValidator;
    private readonly IValidator<CategoryUpdateRequest> cateUpdateValidator;
    private readonly IValidator<CategorySortRequest> cateSortValidator;

    public CategoryController(IListeningRepository repository, ListeningDomainService domainService,
        ListeningDbContext ctx, IValidator<CategoryAddRequest> cateAddValidator,
        IValidator<CategoryUpdateRequest> cateUpdateValidator,
        IValidator<CategorySortRequest> cateSortValidator)
    {
        this.repository = repository;
        this.domainService = domainService;
        this.ctx = ctx;
        this.cateAddValidator = cateAddValidator;
        this.cateUpdateValidator = cateUpdateValidator;
        this.cateSortValidator = cateSortValidator;
    }

    [HttpGet]
    public async Task<Category[]> FindAll()
    {
        return await repository.GetCategoriesAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category?>> FindById(string id)
    {
        var c = await repository.FindCategoryByIdAsync(id);
        if (c == null)
        {
            return NotFound("id不存在");
        }
        return c;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Add(CategoryAddRequest req)
    {
        var res = cateAddValidator.Validate(req);
        if (!res.IsValid)
        {
            return BadRequest(res.SumErrors());
        }
        Category c = await domainService.AddCategoryAsync(req.Name, req.CoverUrl);
        ctx.Add(c);
        return c.Id;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, CategoryUpdateRequest req)
    {
        var validateRes = cateUpdateValidator.Validate(req);
        if (!validateRes.IsValid)
        {
            return BadRequest(validateRes.SumErrors());
        }
        var category = await repository.FindCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound("id不存在");
        }
        category.ChangeCategoryName(req.Name);
        category.ChangeCover(req.CoverUrl);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var category = await repository.FindCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound("id不存在");
        }
        category.SoftDelete();
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Sort(CategorySortRequest req)
    {
        var validateRes = cateSortValidator.Validate(req);
        if (!validateRes.IsValid)
        {
            return BadRequest(validateRes.SumErrors());
        }
        await domainService.SortCategoriesAsync(req.SortedIds);
        return Ok();
    }
}
