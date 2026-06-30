using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

using SalesManagement.Api.Infrastructure.Persistence;
using SalesManagement.Api.Domain.Entities;
using SalesManagement.Api.Features.Products.Dtos;
using SalesManagement.Api.Features.Products.Requests;
using SalesManagement.Api.Features.Products.Responses;
using SalesManagement.Api.Shared.Dtos;
using SalesManagement.Api.Shared.Extensions;
using SalesManagement.Api.Shared.Messages;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Products;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder group)
    {
        group.RequireAuthorization();

        group.MapGet("/", GetProducts);
        group.MapGet("/{id:guid}", GetProduct);
        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:guid}", UpdateProduct);
        group.MapDelete("/{id:guid}", DeleteProduct);

        group.MapGet("/lookup", GetProductLookup);

        return group;
    }

    //商品一覧
    private static async Task<IResult> GetProducts(
        [AsParameters] ProductSearchRequest request,
        AppDbContext db)
    {
        var query = db.Products
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(x =>
                x.ProductCode.Contains(request.Keyword) ||
                x.ProductName.Contains(request.Keyword));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.ProductCode)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ProductListItemDto
            {
                Id = x.Id,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice
            })
            .ToListAsync();

        return Results.Ok(
            new PagedResult<ProductListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
    }

    //商品取得
    private static async Task<IResult> GetProduct(
        Guid id,
        AppDbContext db)
    {
        var product = await db.Products
                                .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductEditDto
            {
                Id = x.Id,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Unit = x.Unit,
                Remarks = x.Remarks,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(product);
    }

    //商品登録
    private static async Task<IResult> CreateProduct(
        CreateProductRequest request,
        IValidator<CreateProductRequest> validator,
        ClaimsPrincipal user,
        AppDbContext db)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var userId = user.GetUserId();

        var sequenceValue = await db.Database
            .SqlQuery<long>($@"SELECT nextval('product_code_seq') AS ""Value""")
            .FirstAsync();

        var productCode = $"P{sequenceValue:D6}";

        var product = new Product
        {
            Id = Guid.NewGuid(),

            ProductCode = productCode,
            ProductName = request.ProductName,
            UnitPrice = request.UnitPrice!.Value,
            Unit = request.Unit,
            Remarks = request.Remarks,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId,

            IsDeleted = false
        };

        db.Products.Add(product);

        await db.SaveChangesAsync();

        return Results.Created(
            $"/products/{product.Id}",
            new CreateProductResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode
            });
    }

    //商品更新
    private static async Task<IResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        IValidator<UpdateProductRequest> validator,
        ClaimsPrincipal user,
        AppDbContext db)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var product = await db.Products
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (product is null)
        {
            return Results.NotFound(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.NotFound
                });
        }

        if (product.UpdatedAt != request.UpdatedAt)
        {
            return Results.Conflict(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.ConcurrencyConflict
                });
        }

        var userId = user.GetUserId();

        product.ProductName = request.ProductName;
        product.UnitPrice = request.UnitPrice!.Value;
        product.Unit = request.Unit;
        product.Remarks = request.Remarks;

        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = userId;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }    

    //商品削除
    private static async Task<IResult> DeleteProduct(
        Guid id,
        ClaimsPrincipal user,
        AppDbContext db)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (product is null)
        {
            return Results.NotFound(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.NotFound
                });
        }

        var userId = user.GetUserId();

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = userId;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> GetProductLookup(AppDbContext db)
    {
        var items = await db.Products
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ProductCode)
            .Select(x => new ProductLookupDto
            {
                Id = x.Id,
                Code = x.ProductCode,
                Name = x.ProductName,
                UnitPrice = x.UnitPrice,
                Unit = x.Unit
            })
            .ToListAsync();

        return Results.Ok(items);
    }  
}