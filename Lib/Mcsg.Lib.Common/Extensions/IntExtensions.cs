namespace Mcsg.Lib.Common.Extensions;

public static class IntExtensions
{
    public static string KiloFormat(this int num)
    {
        if (num >= 100000000)
            return (num / 1000000).ToString("#,0M");

        if (num >= 10000000)
            return (num / 1000000).ToString("0.#") + "M";

        if (num >= 100000)
            return KiloFormat(num / 1000) + "K";

        if (num >= 10000)
            return (num / 1000D).ToString("0.#") + "K";

        return num.ToString("#,0");
    }

    public static float Percentage(this float num, float rate)
    {
        return ((float)num / 100) * rate;
    }
    public static double Percentage(this double num, double rate)
    {
        return ((double)num / 100) * rate;
    }
    public static decimal Percentage(this decimal num, decimal rate)
    {
        return ((decimal)num / 100) * rate;
    }
    public static int Percentage(this int num, int rate)
    {
        return ((int)num / 100) * rate;
    }
    public static long Percentage(this long num, long rate)
    {
        return ((long)num / 100) * rate;
    }
}
