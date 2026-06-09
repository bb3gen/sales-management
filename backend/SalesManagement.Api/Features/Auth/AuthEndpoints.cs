using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;

using SalesManagement.Api.Domain.Auth;
using SalesManagement.Api.Domain.Users;
using SalesManagement.Api.Infrastructure.Persistence;

namespace SalesManagement.Api.Features.Auth;

public static class AuthEndpoints
{
    // エンドポイントの設定
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        // OTPの送信
        group.MapPost("/send-otp", SendOtp);

        // OTPの検証+トークンを送信
        group.MapPost("/verify-otp", VerifyOtp);

        // 新しいトークンを送信
        group.MapPost("/refresh", Refresh);

        // ログアウト
        group.MapPost("/logout", Logout).RequireAuthorization();

        return group;
    }

    // ワンタイムパスワードの送信
    private static async Task<IResult> SendOtp(
        SendOtpRequest request,
        AppDbContext db,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Auth");

        // メールアドレスに該当するユーザを取得
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        // ユーザが存在しない場合は登録
        if (user == null)
        {
            // ユーザを準備
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            // ユーザにレコードを追加
            db.Users.Add(user);
        }

        // ワンタイムパスワードを生成
        var otp = OtpGenerator.Generate();

        // 新規レコードを用意
        var otpCode = new OtpCode
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CodeHash = HashHelper.Sha256(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10), // 有効期限10分
            FailedCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        // テーブルに追加
        db.OtpCodes.Add(otpCode);

        // データを保存
        await db.SaveChangesAsync();

        // ここらへんでメールを送る予定
        logger.LogInformation("OTP for {Email}: {Otp}", request.Email, otp);

        return Results.Ok();
    }

    // ワンタイムパスワードの検証 OKの時はアクセストークンとリフレッシュトークンを返す
    private static async Task<IResult> VerifyOtp(
        VerifyOtpRequest request,
        AppDbContext db, 
        JwtTokenService tokenService)
    {
        // メールアドレスに該当するユーザを取得
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        // ユーザが存在しない場合はエラー
        if (user is null)
            return Results.BadRequest();

        // 6桁数値のハッシュキー
        var hash = HashHelper.Sha256(request.Code);

        // ワンタイムパスワードの履歴(未使用)を検索
        var otp = await db.OtpCodes
                            .Where(x => x.UserId == user.Id && x.UsedAt == null)
                            .OrderByDescending(x => x.CreatedAt)
                            .FirstOrDefaultAsync();

        // 該当なし
        if (otp is null)
            return Results.BadRequest();

        // 期限切れ
        if (otp.ExpiresAt < DateTime.UtcNow)
            return Results.BadRequest();
 
        // 失敗回数
        if (otp.FailedCount >= 5)
            return Results.BadRequest();
        
        // OTPが異なる場合、失敗回数をカウントアップ
        if (otp.CodeHash != hash)
        {
            otp.FailedCount++;

            await db.SaveChangesAsync();

            return Results.BadRequest();
        }

        // ワンタイムパスワードを使用済に
        otp.UsedAt = DateTime.UtcNow;

        // データを保存
        await db.SaveChangesAsync();

        // アクセストークンを作成
        var accessToken = tokenService.CreateAccessToken(user.Id, user.Email);

        // リフレッシュトークンを作成
        var refreshToken = tokenService.CreateRefreshToken();

        // リフレッシュトークンを追加
        db.RefreshTokens.Add(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = HashHelper.Sha256(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30) //有効期間は30日
            });

        // データを保存
        await db.SaveChangesAsync();

        // 成功
        return Results.Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    // リフレッシュトークンの更新
    private static async Task<IResult> Refresh(
        RefreshTokenRequest request,
        AppDbContext db,
        JwtTokenService tokenService)
    {
        // ハッシュキーを取得
        var hash = HashHelper.Sha256(request.RefreshToken);

        // リフレッシュトークンを探す
        var token = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash);

        // レコードなし
        if (token is null)
            return Results.Unauthorized();

        // 削除済み
        if (token.RevokedAt != null)
            return Results.Unauthorized();

        // 期限切れ
        if (token.ExpiresAt < DateTime.UtcNow)
            return Results.Unauthorized();

        // ユーザを取得
        var user = await db.Users.FirstAsync(x => x.Id == token.UserId);

        // 新しいアクセストークンを作成
        var accessToken = tokenService.CreateAccessToken(user.Id, user.Email);

        // 新しいリフレッシュトークンを作成
        var newRefreshToken = tokenService.CreateRefreshToken();

        // 旧トークンを無効化
        token.RevokedAt = DateTime.UtcNow;

        // 新トークンのレコード追加
        db.RefreshTokens.Add(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = HashHelper.Sha256(newRefreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            });

        // データを保存
        await db.SaveChangesAsync();            

        // 成功
        return Results.Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        });
    }

    // ログアウト
    private static async Task<IResult> Logout(
        ClaimsPrincipal user, //[Authorize]を付けたエンドポイントなら取得できる。
        LogoutRequest request,
        AppDbContext db)
    {
        // リフレッシュトークンのハッシュキー
        var hash = HashHelper.Sha256(request.RefreshToken);

        // リフレッシュトークンのレコードを取得
        var token = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash);
        // 存在しない場合はOK
        if (token is null)
            return Results.Ok();

        if (token.UserId != user.GetUserId())
            return Results.Forbid(); //403 Forbidden

        // リフレッシュトークンを無効化
        token.RevokedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}