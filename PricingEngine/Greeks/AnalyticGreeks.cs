using System;
using PricingEngine.Models;
using PricingEngine.Utils;
using static System.Math;

namespace PricingEngine.Greeks
{
    public class AnalyticGreeks : IGreekCalculator
    {
        public GreekResult Compute(Option option, Market market)
        {
            if (option is not EuropeanOption eu)
                throw new NotSupportedException("Analytic Greeks ne supportent que les options européennes.");

            return ComputeInternal(eu, market);
        }

        public static GreekResult Compute(EuropeanOption opt, Market mkt)
        {
            return ComputeInternal(opt, mkt);
        }

        private static GreekResult ComputeInternal(EuropeanOption opt, Market mkt)
        {
            double S     = mkt.Spot;
            double K     = opt.Strike;
            double r     = mkt.Rate;
            double q     = mkt.DividendYield;
            double sigma = mkt.Vol;
            double T     = opt.Maturity;

            double sqrtT = Sqrt(T);

            double d1 = (Log(S / K) + (r - q + 0.5 * sigma * sigma) * T)
                        / (sigma * sqrtT);

            double d2 = d1 - sigma * sqrtT;

            double Nd1   = MathUtil.NormCdf(d1);
            double Nd2   = MathUtil.NormCdf(d2);
            double nd1   = MathUtil.NormPdf(d1);
            double discR = Exp(-r * T);
            double discQ = Exp(-q * T);

            double delta = opt.Type == OptionType.Call
                ? discQ * Nd1
                : discQ * (Nd1 - 1.0);

            double gamma = discQ * nd1 / (S * sigma * sqrtT);

            double vega = S * discQ * nd1 * sqrtT;

            double thetaFirst =
                - S * discQ * nd1 * sigma / (2.0 * sqrtT);

            double theta = opt.Type == OptionType.Call
                ? thetaFirst
                  - r * K * discR * Nd2
                  + q * S * discQ * Nd1
                : thetaFirst
                  + r * K * discR * MathUtil.NormCdf(-d2)
                  - q * S * discQ * MathUtil.NormCdf(-d1);

            double rho = opt.Type == OptionType.Call
                ? K * T * discR * Nd2
                : -K * T * discR * MathUtil.NormCdf(-d2);

            double vanna = discQ * nd1 * sqrtT * (1 - d1 / (sigma * sqrtT));

            double vomma = vega * d1 * d2 / sigma;

            double zomma = gamma * (d1 * d2 - 1) / sigma;

            return new GreekResult
            {
                Delta = delta,
                Gamma = gamma,
                Vega  = vega,
                Theta = theta,
                Rho   = rho,
                Vanna = vanna,
                Vomma = vomma,
                Zomma = zomma
            };
        }
    }
}
