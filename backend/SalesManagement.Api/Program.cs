using System.Text;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using SalesManagement.Api.Infrastructure.Persistence;
using SalesManagement.Api.Shared.Extensions;

using SalesManagement.Api.Features.Users;
using SalesManagement.Api.Features.Auth;
using SalesManagement.Api.Features.Auth.Services;
using SalesManagement.Api.Features.Customers;

using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention(); //パスカルケースをスネークケースに自動変換
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// DI登録
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Authentication登録
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),

                ClockSkew = TimeSpan.Zero //時刻の許容誤差なし
            };
    });

// Authorization登録
builder.Services.AddAuthorization();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ミドルウェア追加
app.UseHttpsRedirection();

app.UseCors("VueApp");

app.UseAuthentication();
app.UseAuthorization();

// エンドポイントの設定
app.MapGet("/health", () =>
{
    return Results.Ok("OK");
});

var api = app.MapGroup("/api");

api.MapGroup("/auth")
    .MapAuthEndpoints();

api.MapGroup("/users")
    .RequireAuthorization()
    .MapUserEndpoints();

api.MapGroup("/customers")
    .RequireAuthorization()
    .MapCustomerEndpoints();

api.MapGet("/me", [Authorize] (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        UserId = user.GetUserId(), //user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        Email = user.GetEmail()  //user.FindFirst(ClaimTypes.Email)?.Value
    });
});

app.Run();
