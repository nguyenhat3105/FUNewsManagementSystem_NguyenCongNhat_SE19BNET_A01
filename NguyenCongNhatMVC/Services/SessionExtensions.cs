using System.Text.Json;
using NguyenCongNhatMVC.Models;

namespace NguyenCongNhatMVC.Services;

public static class SessionExtensions
{
    private const string UserKey = "CurrentUser";

    public static void SetUser(this ISession session, AuthenticatedUser user)
    {
        session.SetString(UserKey, JsonSerializer.Serialize(user));
    }

    public static AuthenticatedUser? GetUser(this ISession session)
    {
        var json = session.GetString(UserKey);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AuthenticatedUser>(json);
    }

    public static bool IsInRole(this ISession session, string role)
    {
        return session.GetUser()?.Role == role;
    }
}
