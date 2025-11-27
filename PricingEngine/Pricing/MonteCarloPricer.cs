using System;
using PricingEngine.Models;
using PricingEngine.Models.Payoffs;
using static System.Math;

namespace PricingEngine.Pricing
{
    public class MonteCarloPricer : IOptionPricer
    {
        private readonly int _paths;
        private readonly bool _antithetic;
        private static readonly Random _rng = new Random();

        public MonteCarloPricer(int paths = 100_000, bool antithetic = true)
        {
            _paths = paths;
            _antithetic = antithetic;
        }

        public double Price(Option option, Market market)
        {
            if (option.Style != OptionStyle.European)
                throw new NotSupportedException("Monte Carlo supports only European options.");

            return PriceEuropean((EuropeanOption)option, market);
        }
        private double PriceEuropean(EuropeanOption opt, Market mkt)
        {
            double S0  = mkt.Spot;
            double K   = opt.Strike;
            double r   = mkt.Rate;
            double q   = mkt.DividendYield;
            double vol = mkt.Vol;
            double T   = opt.Maturity;

            double drift = (r - q - 0.5 * vol * vol) * T;
            double diff  = vol * Sqrt(T);

            int n = _paths;
            int effPaths = _antithetic ? n / 2 : n;

            double payoffSum = 0.0;

            for (int i = 0; i < effPaths; i++)
            {
                double z = NextGaussian();

                double ST1 = S0 * Exp(drift + diff * z);
                double payoff1 = opt.Payoff.Evaluate(ST1);

                if (_antithetic)
                {
                    double ST2 = S0 * Exp(drift - diff * z);
                    double payoff2 = opt.Payoff.Evaluate(ST2);

                    payoffSum += payoff1 + payoff2;
                }
                else
                {
                    payoffSum += payoff1;
                }
            }

            double mean = payoffSum / n;
            return Exp(-r * T) * mean;
        }
        private static double NextGaussian()
        {
            double u1 = 1.0 - _rng.NextDouble();
            double u2 = 1.0 - _rng.NextDouble();
            return Sqrt(-2.0 * Log(u1)) * Cos(2.0 * PI * u2);
        }
    }
}
