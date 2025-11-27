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
                GreekMethod.Analytic =>
                    new AnalyticGreeks(),

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
