using System;
using PricingEngine.Models;
using PricingEngine.Pricing;

namespace PricingEngine.Greeks
{
    public enum GreekMethod
    {
        Analytic,
        FiniteDifference
    }

    /// <summary>
    /// Factory to create Greek calculators (Analytic or Finite Difference)
    /// using any pricing backend (BS, Binomial, MonteCarlo...)
    /// </summary>
    public static class GreekFactory
    {
        public static IGreekCalculator Create(
            GreekMethod method,
            PricingMethod pricingMethod = PricingMethod.BlackScholes,
            double hS = 0.5,
            double hV = 0.01,
            double hT = 0.2,
            double hR = 0.1)
        {
            return method switch
            {
                // ==============================
                //  ANALYTIC GREEKS (BS only)
                // ==============================
                GreekMethod.Analytic =>
                    new AnalyticGreeks(),


                // =============================================
                //  FINITE DIFFERENCE GREEKS (ANY pricing model)
                // =============================================
                GreekMethod.FiniteDifference =>
                    new FiniteDifferenceGreeks(
                        pricer: PricerFactory.Create(pricingMethod),
                        hS: hS,
                        hV: hV,
                        hT: hT,
                        hR: hR
                    ),

                _ => throw new NotSupportedException(
                    $"Unsupported Greek method: {method}")
            };
        }
    }
}
