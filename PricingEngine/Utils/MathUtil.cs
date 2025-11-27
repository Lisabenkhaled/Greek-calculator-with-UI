using System;
using static System.Math;

namespace PricingEngine.Utils
{
    public static class MathUtil
    {
        private const double INV_SQRT_2PI = 0.39894228040143267793994605993438;

        public static double NormPdf(double x)
        {
            return INV_SQRT_2PI * Exp(-0.5 * x * x);
        }
        public static double NormCdf(double x)
        {
            double sign = 1;
            if (x < 0) sign = -1;
            x = Abs(x) / Sqrt(2.0);

            double t = 1.0 / (1.0 + 0.3275911 * x);

            double y =
                1.0
                - ((((1.061405429 * t
                - 1.453152027) * t
                + 1.421413741) * t
                - 0.284496736) * t
                + 0.254829592) *
                t * Exp(-x * x);

            return 0.5 * (1.0 + sign * y);
        }
    }
}
