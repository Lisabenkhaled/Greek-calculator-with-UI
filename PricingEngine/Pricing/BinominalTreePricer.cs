using System;
using PricingEngine.Models;
using PricingEngine.Models.Payoffs;
using static System.Math;

namespace PricingEngine.Pricing
{
    /// <summary>
    /// Cox–Ross–Rubinstein (CRR) Binomial Tree Pricer.
    /// Uses continuous dividend yield q.
    /// Fully compatible with IPayoff and IOptionPricer.
    /// </summary>
    public class BinomialTreePricer : IOptionPricer
    {
        private readonly int _steps;

        public BinomialTreePricer(int steps = 2000)
        {
            _steps = steps;
        }

        // Interface method
        public double Price(Option option, Market market)
        {
            return option.Style switch
            {
                OptionStyle.European => PriceEuropean((EuropeanOption)option, market, _steps),
                OptionStyle.American => PriceAmerican((AmericanOption)option, market, _steps),
                _ => throw new NotSupportedException("Unsupported option style in Binomial Tree")
            };
        }


        // ============================================================
        // 1. BUILD PARAMETERS
        // ============================================================
        public static TreeParameters BuildCRRParameters(Market mkt, double T, int steps)
        {
            var tp = new TreeParameters(steps);

            double dt = T / steps;
            tp.Dt = dt;

            double sigma = mkt.Vol;
            double r = mkt.Rate;
            double q = mkt.DividendYield;

            tp.UpFactor   = Exp(sigma * Sqrt(dt));
            tp.DownFactor = 1.0 / tp.UpFactor;

            tp.DiscountFactor = Exp(-r * dt);

            tp.RiskNeutralProb =
                (Exp((r - q) * dt) - tp.DownFactor) /
                (tp.UpFactor - tp.DownFactor);

            return tp;
        }


        // ============================================================
        // 2. EUROPEAN OPTION USING PAYOFF INTERFACE
        // ============================================================
        public static double PriceEuropean(EuropeanOption opt, Market mkt, int steps)
        {
            var tp = BuildCRRParameters(mkt, opt.Maturity, steps);

            double S0 = mkt.Spot;
            double u = tp.UpFactor;
            double d = tp.DownFactor;
            double p = tp.RiskNeutralProb;
            double disc = tp.DiscountFactor;

            double[] values = new double[steps + 1];

            // Terminal payoff using IPayoff
            for (int i = 0; i <= steps; i++)
            {
                double ST = S0 * Pow(u, steps - i) * Pow(d, i);
                values[i] = opt.Payoff.Evaluate(ST);
            }

            // Backward induction
            for (int j = steps - 1; j >= 0; j--)
            {
                for (int i = 0; i <= j; i++)
                {
                    values[i] = disc * (p * values[i] + (1 - p) * values[i + 1]);
                }
            }

            return values[0];
        }


        // ============================================================
        // 3. AMERICAN OPTION USING PAYOFF INTERFACE
        // ============================================================
        public static double PriceAmerican(AmericanOption opt, Market mkt, int steps)
        {
            var tp = BuildCRRParameters(mkt, opt.Maturity, steps);

            double S0 = mkt.Spot;
            double u = tp.UpFactor;
            double d = tp.DownFactor;
            double p = tp.RiskNeutralProb;
            double disc = tp.DiscountFactor;

            double[] values = new double[steps + 1];

            // Terminal payoff
            for (int i = 0; i <= steps; i++)
            {
                double ST = S0 * Pow(u, steps - i) * Pow(d, i);
                values[i] = opt.Payoff.Evaluate(ST);
            }

            // Backward induction with early exercise
            for (int j = steps - 1; j >= 0; j--)
            {
                for (int i = 0; i <= j; i++)
                {
                    double continuation = disc * (p * values[i] + (1 - p) * values[i + 1]);

                    double Sj = S0 * Pow(u, j - i) * Pow(d, i);
                    double intrinsic = opt.Payoff.Evaluate(Sj);

                    values[i] = Max(continuation, intrinsic);
                }
            }

            return values[0];
        }
    }
}
