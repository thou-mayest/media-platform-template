using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.Categories.Commands.CreateCategory;
using Posts.Application.Categories.Commands.DeleteCategory;
using Posts.Application.Categories.Commands.UpdateCategory;
using Posts.Application.Categories.Queries.GetCategories;
using Posts.Application.Categories.Queries.GetCategoryById;
using Posts.Presentation.Authorization;
using SharedKernal.Extensions;

namespace Posts.Presentation.Categories;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return result.Match(categories => Ok(categories));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return result.Match(category => Ok(category));
    }

    [HttpPost]
    [Authorize(Policy = PostsPolicies.RequireAdmin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateCategoryCommand(request.Name), cancellationToken);
        return result.Match(id => CreatedAtAction(nameof(GetById), new { id }, id));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PostsPolicies.RequireAdmin)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateCategoryCommand(id, request.Name), cancellationToken);
        return result.Match(NoContent());
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PostsPolicies.RequireAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return result.Match(NoContent());
    }
}
