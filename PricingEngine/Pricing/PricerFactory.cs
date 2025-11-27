using System;
using PricingEngine.Models;

namespace PricingEngine.Pricing
{
    public enum PricingMethod
    {
        BlackScholes,
        Binomial,
        MonteCarlo,
        LSMMonteCarlo
    }

    public static class PricerFactory
    {
        public static IOptionPricer Create(
            PricingMethod method,
            int steps = 2000,
            int mcPaths = 1_000_000,
            bool antithetic = true)
        {
            return method switch
            {
                PricingMethod.BlackScholes =>
                    new BlackScholesPricer(),

                PricingMethod.Binomial =>
                    new BinomialTreePricer(steps),

                PricingMethod.MonteCarlo =>
                    new MonteCarloPricer(mcPaths, antithetic),

                PricingMethod.LSMMonteCarlo =>
                    new LSMMonteCarloPricer(
                        paths: 200_000,
                        steps: 50),

                _ => throw new NotSupportedException($"Pricing method {method} is not supported")
            };
        }
    }
}
