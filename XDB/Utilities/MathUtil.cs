using System;

namespace XDB.Utilities
{
    public class MathUtil
    {
        public static double FToCelsius(double f)
        {
            return Math.Round(5.0 / 9.0 * (f - 32));
        }

        public static double CelsiusToF(double c)
        {
            return Math.Round(((9.0 / 5.0) * c) + 32);
        }

        public static double Multiply(double x, double y)
        {
            return Math.Round(x * y);
        }

        public static double Divide(double x, double y)
        {
            return Math.Round(x / y);
        }

        public static double Add(double x, double y)
        {
            return Math.Round(x + y);
        }

        public static double Sub(double x, double y)
        {
            return Math.Round(x - y);
        }
    }
}
