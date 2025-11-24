using PricingEngine.Models;
using PricingEngine.Pricing;

namespace PricingEngine.Greeks
{
    public static class GreekEngine
    {
        public static GreekResult Compute(
            Option option,
            Market market,
            GreekMethod greekMethod,
            PricingMethod pricingMethod = PricingMethod.BlackScholes,
            double hS = 0.5,
            double hV = 0.01,
            double hT = 0.2,
            double hR = 0.01)
        {
            // Create the correct Greek calculator
            var calculator = GreekFactory.Create(
                method: greekMethod,
                pricingMethod: pricingMethod,
                hS: hS,
                hV: hV,
                hT: hT,
                hR: hR
            );

            // Compute using analytic or FD implementation
            return calculator.Compute(option, market);
        }
    }
}
