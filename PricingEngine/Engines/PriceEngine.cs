using PricingEngine.Models;
using PricingEngine.Pricing;

namespace PricingEngine.Engines
{
    public static class PriceEngine
    {
        public static double Price(Option opt, Market mkt, PricingMethod method)
        {
            var pricer = PricerFactory.Create(method);
            return pricer.Price(opt, mkt);
        }
    }
}
