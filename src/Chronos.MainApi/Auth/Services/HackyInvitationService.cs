using System.Text;

namespace Chronos.MainApi.Auth.Services;

// TODO Remove this bullshit
public class HackyInvitationService
{
    private const long Prime = 22697L;
    private const long TopMultiplier = 10_000L;
    private const string Map = "0123456789abcdefghijklmnopqrstuvwxyz";

    private readonly Random _random = new();

    public bool VerifyInviteCode(string code)
    {
        var num = 0L;
        var arr = code.ToCharArray().Reverse().ToArray();

        foreach (var c in arr)
        {
            num *= Map.Length;
            num += Map.IndexOf(c);
        }

        return num % Prime == 0;
    }

    public string GenerateInviteCode()
    {
        var builder = new StringBuilder();
        var multiplier = _random.NextInt64(TopMultiplier);
        var num = multiplier * Prime;
        var radix = Map.Length;

        while (num > 0)
        {
            builder.Append(Map[(int)num % radix]);
            num /= radix;
        }

        return builder.ToString();
    }
}