using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

using SalesManagement.Api.Infrastructure.Persistence;
using SalesManagement.Api.Domain.Entities;
using SalesManagement.Api.Features.Customers.Dtos;
using SalesManagement.Api.Features.Customers.Requests;
using SalesManagement.Api.Features.Customers.Responses;
using SalesManagement.Api.Shared.Dtos;
using SalesManagement.Api.Shared.Extensions;
using SalesManagement.Api.Shared.Messages;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Customers;

public static class CustomerEndpoints
{
    // エンドポイントの設定
    public static RouteGroupBuilder MapCustomerEndpoints(this RouteGroupBuilder group)
    {
        //group.RequireAuthorization();
        group.MapGet("/", GetCustomers);
        group.MapPost("/", CreateCustomer);
        group.MapGet("/{id:guid}", GetCustomer);
        group.MapPut("/{id:guid}", UpdateCustomer);
        group.MapDelete("/{id:guid}", DeleteCustomer);
       group.MapGet("/lookup", GetCustomerLookup);

        return group;
    }

    // 顧客リストを検索、取得
    private static async Task<IResult> GetCustomers(
        [AsParameters] CustomerSearchRequest request,
        AppDbContext db)
    {
        var query = db.Customers
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.CustomerCode))
        {
            query = query.Where(x =>
                x.CustomerCode.Contains(request.CustomerCode));
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            // query = query.Where(x =>
            //     x.Name.Contains(request.CustomerName));

            // 山田, やまだ, YAMADA, yamada を検索できるようになる!?
            query = query.Where(x =>
                EF.Functions.ILike(
                    x.CustomerName,
                    $"%{request.CustomerName}%"));
        }

        var totalCount = await query.CountAsync();

        // 最大100件までに制限
        request.PageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderBy(x => x.CustomerCode)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new CustomerListItemDto
            {
                Id = x.Id,
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email
            })
            .ToListAsync();

        return Results.Ok(new PagedResult<CustomerListItemDto>
        {
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = items
        });
    }

    // 顧客を新規登録
    public static async Task<IResult> CreateCustomer(
        CreateCustomerRequest request,
        IValidator<CreateCustomerRequest> validator,
        AppDbContext db,
        ClaimsPrincipal user)        
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var userId = user.GetUserId();

        var sequenceNo = await db.Database
            .SqlQuery<long>($@"SELECT nextval('customer_code_seq') AS ""Value""")
            .FirstAsync();

        var customerCode = $"C{sequenceNo:D6}";

        var customer = new Customer
        {
            Id = Guid.NewGuid(),

            CustomerCode = customerCode,
            CustomerName = request.CustomerName,
            CustomerKana = request.CustomerKana,
            PostalCode = request.PostalCode,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId,

            IsDeleted = false
        };
        
        db.Customers.Add(customer);

        await db.SaveChangesAsync();

        return Results.Created(
            $"/customers/{customer.Id}",
            new CreateCustomerResponse
            {
                Id = customer.Id,
                CustomerCode = customer.CustomerCode
            });       
    }

    //顧客の取得
    private static async Task<IResult> GetCustomer(Guid id, AppDbContext db)
    {
        var customer = await db.Customers
                                .AsNoTracking()
                                .Where(x => !x.IsDeleted)
                                .Select(x => new CustomerEditDto
                                {
                                    Id = x.Id,
                                    CustomerCode = x.CustomerCode,
                                    CustomerName = x.CustomerName,
                                    CustomerKana = x.CustomerKana,
                                    PostalCode = x.PostalCode,
                                    Address = x.Address,
                                    PhoneNumber = x.PhoneNumber,
                                    Email = x.Email,
                                    UpdatedAt = x.UpdatedAt
                                })
                                .FirstOrDefaultAsync(x => x.Id == id);

        if (customer is null)
            return Results.NotFound();

        return Results.Ok(customer);
    }

    //顧客の編集
    public static async Task<IResult> UpdateCustomer(
        Guid id,
        UpdateCustomerRequest request,
        IValidator<UpdateCustomerRequest> validator,
        AppDbContext db,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var customer = await db.Customers.FirstOrDefaultAsync(x =>x.Id == id && !x.IsDeleted);

        if (customer is null)
        {
            return Results.NotFound();
        }

        if (customer.UpdatedAt != request.UpdatedAt)
        {
            return Results.Conflict(new ApiErrorResponse
            {
                Message = ErrorMessages.ConcurrencyConflict
            });
        }        

        customer.CustomerName = request.CustomerName;
        customer.CustomerKana = request.CustomerKana;
        customer.PostalCode = request.PostalCode;
        customer.Address = request.Address;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;

        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = user.GetUserId();

        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    //顧客の論理削除
    private static async Task<IResult> DeleteCustomer(Guid id, ClaimsPrincipal user, AppDbContext db)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (customer is null)
        {
            return Results.NotFound();
        }

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = user.GetUserId();

        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> GetCustomerLookup(AppDbContext db)
    {
        var items = await db.Customers
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.CustomerCode)
            .Select(x => new LookupItemDto
            {
                Id = x.Id,
                Code = x.CustomerCode,
                Name = x.CustomerName
            })
            .ToListAsync();

        return Results.Ok(items);
    }    
}
