using Chronos.Shared.Exceptions;

namespace Chronos.MainApi.Auth.Validation;

public static class AvatarUrlValidator
{
    private const int MaxLength = 2048;
    private static readonly string[] AllowedSchemes = ["http", "https"];

    public static void ValidateAvatarUrl(string? avatarUrl)
    {
        // Null or empty is allowed - avatar is optional
        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            return;
        }

        if (avatarUrl.Length > MaxLength)
        {
            throw new BadRequestException($"Avatar URL must not exceed {MaxLength} characters");
        }

        if (!Uri.TryCreate(avatarUrl, UriKind.Absolute, out var uri))
        {
            throw new BadRequestException("Avatar URL format is invalid");
        }

        if (!AllowedSchemes.Contains(uri.Scheme.ToLowerInvariant()))
        {
            throw new BadRequestException("Avatar URL must use HTTP or HTTPS protocol");
        }
    }
}
