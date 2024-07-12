namespace Mcsg.Lib.Common.Helpers;

public static class NumberHelper
{
    public static int RandomNumber(int num1, int num2)
    {
        Random rnd = new Random();
        return rnd.Next(num1, num2);
    }
}
