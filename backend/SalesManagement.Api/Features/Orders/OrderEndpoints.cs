using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Api.Domain.Entities;
using SalesManagement.Api.Domain.Enums;
using SalesManagement.Api.Features.Orders.Dtos;
using SalesManagement.Api.Features.Orders.Requests;
using SalesManagement.Api.Infrastructure.Persistence;
using SalesManagement.Api.Shared.Dtos;
using SalesManagement.Api.Shared.Extensions;
using SalesManagement.Api.Shared.Messages;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Orders;
public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetOrders);
        group.MapGet("/{id:guid}", GetOrder);
        group.MapPost("/", CreateOrder);
        group.MapPut("/{id:guid}", UpdateOrder);
        group.MapDelete("/{id:guid}", DeleteOrder);

        return group;
    }

    //注文一覧
    private static async Task<IResult> GetOrders(
        [AsParameters] OrderSearchRequest request,
        IValidator<OrderSearchRequest> validator,
        AppDbContext db)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var query = db.Orders
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.OrderNumber))
        {
            query = query.Where(x =>
                x.OrderNumber.Contains(request.OrderNumber));
        }

        if (request.CustomerId.HasValue)
        {
            query = query.Where(x =>
                x.CustomerId == request.CustomerId.Value);
        }

        if (request.OrderDateFrom.HasValue)
        {
            query = query.Where(x =>
                x.OrderDate >= request.OrderDateFrom.Value);
        }

        if (request.OrderDateTo.HasValue)
        {
            query = query.Where(x =>
                x.OrderDate <= request.OrderDateTo.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.OrderDate)
            .ThenByDescending(x => x.OrderNumber)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new OrderListItemDto
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                OrderDate = x.OrderDate,
                CustomerName = x.Customer.CustomerName,
                TotalAmount = x.TotalAmount,
                Status = x.Status
            })
            .ToListAsync();

        return Results.Ok(new PagedResult<OrderListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    //注文取得
    private static async Task<IResult> GetOrder(
        Guid id,
        AppDbContext db)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Where(x => !x.IsDeleted)
            .Select(x => new OrderEditDto
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                OrderDate = x.OrderDate,
                CustomerId = x.CustomerId,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                UpdatedAt = x.UpdatedAt,

                Details = x.OrderDetails
                    .OrderBy(d => d.LineNo)
                    .Select(d => new OrderDetailDto
                    {
                        Id = d.Id,
                        LineNo = d.LineNo,

                        ProductId = d.ProductId,

                        ProductCode = d.Product.ProductCode,
                        ProductName = d.Product.ProductName,
                        Unit = d.Product.Unit,

                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (order is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(order);
    }

    //注文登録
    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        AppDbContext db,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var userId = user.GetUserId();
        var orderDate = request.OrderDate!.Value;

        //
        // 顧客存在チェック
        //
        var customerId = request.CustomerId!.Value;

        var customerExists = await db.Customers
            .AnyAsync(x =>
                x.Id == customerId &&
                !x.IsDeleted);

        if (!customerExists)
        {
            return Results.BadRequest(new
            {
                Message = "顧客が存在しません。"
            });
        }

        //
        // 商品取得
        //
        var productIds = request.Details
            .Select(x => x.ProductId!.Value)
            .Distinct()
            .ToList();

        var products = await db.Products
            .Where(x => productIds.Contains(x.Id))
            .Where(x => !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id);

        if (products.Count != productIds.Count)
        {
            return Results.BadRequest(new
            {
                Message = "存在しない商品が含まれています。"
            });
        }

        //
        // 受注番号採番
        //
        var sequenceNo = await db.Database
            .SqlQuery<long>($@"SELECT nextval('order_number_seq') AS ""Value""")
            .FirstAsync();

        var orderNumber = $"O{sequenceNo:D6}";

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.UtcNow;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = orderDate,
                CustomerId = customerId,
                Status = OrderStatus.Draft,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = userId,
                UpdatedBy = userId,
                IsDeleted = false
            };

            var lineNo = 1;

            foreach (var item in request.Details)
            {
                var productId = item.ProductId!.Value;
                var product = products[productId];

                var amount = item.Quantity * product.UnitPrice;

                order.OrderDetails.Add(
                    new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        LineNo = lineNo++,
                        ProductId = productId,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice,
                        Amount = amount,
                        CreatedAt = now,
                        UpdatedAt = now,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        IsDeleted = false
                    });
            }

            order.TotalAmount = order.OrderDetails.Sum(x => x.Amount);

            db.Orders.Add(order);

            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Results.Ok(
                new SaveResultDto
                {
                    Id = order.Id
                });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    //注文更新
    private static async Task<IResult> UpdateOrder(
        Guid id,
        UpdateOrderRequest request,
        IValidator<UpdateOrderRequest> validator,
        AppDbContext db,
        ClaimsPrincipal user)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToCamelCaseDictionary());
        }

        var order = await db.Orders
            .Include(x => x.OrderDetails)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (order is null)
        {
            return Results.NotFound(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.NotFound
                });
        }

        if (order.UpdatedAt != request.UpdatedAt)
        {
            return Results.Conflict(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.ConcurrencyConflict
                });
        }

        var customerId = request.CustomerId!.Value;

        var customerExists = await db.Customers
            .AnyAsync(x =>
                x.Id == customerId &&
                !x.IsDeleted);

        if (!customerExists)
        {
            return Results.BadRequest(new
            {
                Message = "顧客が存在しません。"
            });
        }

        var productIds = request.Details
            .Select(x => x.ProductId!.Value)
            .Distinct()
            .ToList();

        var products = await db.Products
            .Where(x => productIds.Contains(x.Id))
            .Where(x => !x.IsDeleted)
            .ToDictionaryAsync(x => x.Id);

        if (products.Count != productIds.Count)
        {
            return Results.BadRequest(new
            {
                Message = "存在しない商品が含まれています。"
            });
        }

        var userId = user.GetUserId();
        var now = DateTime.UtcNow;

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            order.OrderDate = request.OrderDate!.Value;
            order.CustomerId = customerId;
            order.UpdatedAt = now;
            order.UpdatedBy = userId;

            db.OrderDetails.RemoveRange(order.OrderDetails);

            var lineNo = 1;
            var newDetails = new List<OrderDetail>();

            foreach (var item in request.Details)
            {
                var productId = item.ProductId!.Value;
                var product = products[productId];
                var amount = item.Quantity * product.UnitPrice;

                newDetails.Add(
                    new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        LineNo = lineNo++,
                        ProductId = productId,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice,
                        Amount = amount,
                        CreatedAt = now,
                        UpdatedAt = now,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        IsDeleted = false
                    });
            }

            db.OrderDetails.AddRange(newDetails);

            order.TotalAmount = newDetails.Sum(x => x.Amount);

            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Results.NoContent();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    //注文削除
    private static async Task<IResult> DeleteOrder(
        Guid id,
        AppDbContext db,
        ClaimsPrincipal user)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (order is null)
        {
            return Results.NotFound(
                new ApiErrorResponse
                {
                    Message = ErrorMessages.NotFound
                });
        }

        order.IsDeleted = true;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = user.GetUserId();

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
