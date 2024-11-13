using System;

namespace SymbolMaker
{
    public static class HelperClass
    {
        public static double Epsilon = 1e-12;//default small tolerance
        public static double DegToRad = Math.PI / 180.0;
        public static double RadToDeg = 180.0 / Math.PI;

        public static void Direction(int index, out int horizontal, out int vertical)
        {
            switch (index)
            {
                case 1:
                    horizontal = 1;
                    vertical = 1;
                    break;
                case 2:
                    horizontal = -1;
                    vertical = -1;
                    break;
                case 3:
                    horizontal = -1;
                    vertical = 1;
                    break;
                default:
                    horizontal = 1;
                    vertical = -1;
                    break;
            }
        }

        public static bool IsEqual(double d1, double d2)
        {
            return IsEqual(d1, d2, Epsilon);
        }

        public static bool IsEqual(double d1, double d2, double epsilon)
        {
            return IsZero(Math.Abs(d1 - d2), epsilon);
        }

        public static bool IsZero(double d, double epsilon)
        {
            return d >= -epsilon && d <= epsilon;
        }

        public static bool IsZero(double d)
        {
            return IsZero(d, Epsilon);
        }

        public static double CopySign(double a, double b)
        {
            return a * Math.Sign(b);
        }

        public static double NthRoot(double a, int n)
        {
            double r = a;
            double k = a / n;
            while (Math.Abs(r - k) > Epsilon)
            {
                k = r;
                r = (1.0 / n) * ((n - 1) * k + a / Math.Pow(k, n - 1));
            }
            return r;
        }

        public static int Sign(bool flg)
        {
            switch(flg)
            {
                case true:
                    return -1;
                default:
                    return 1;
            }
        }
    }
}
