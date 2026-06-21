namespace SalesManagement.Api.Shared.Validation;

public static class ValidationMessages
{
    public const string Required = "{PropertyName}を入力してください";

    public const string MaxLength =
        "{PropertyName}は{MaxLength}文字以内で入力してください";

    public const string Email =
        "メールアドレスの形式が正しくありません";
}
