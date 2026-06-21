namespace SalesManagement.Api.Shared.Messages;

public static class ErrorMessages
{
    public const string ConcurrencyConflict =
        "他のユーザーによって更新されています。";

    public const string NotFound =
        "データが見つかりません。";

    public const string Unauthorized =
        "認証に失敗しました。";
}
