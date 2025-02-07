using System.Text;

namespace Votr.Core;

public static class Randomizer
{
    private static readonly Random Random;
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GetRandomRoomCode(int length = 8)
    {
        var code = new char[length];
        for (var i = 0; i < length; i++)
        {
            code[i] = Chars[Random.Next(Chars.Length)];
        }

        return new string(code);
    }


    static Randomizer()
    {
        Random = new Random(DateTimeOffset.UtcNow.Microsecond * DateTimeOffset.UtcNow.Second * DateTimeOffset.UtcNow.Minute);
    }
}