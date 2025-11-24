using PricingEngine.Engines;
using PricingEngine.Models;
using PricingEngine.Pricing;
using Shared.DTO;
using Shared.Enums;

namespace Server.Services
{
    public class PricingService
    {
        private readonly OptionFactory _factory;

        public PricingService(OptionFactory factory)
        {
            _factory = factory;
        }

        public double ComputePrice(PricingRequest req)
        {
            Option opt = _factory.CreateOptionFromPricing(req);

            var market = new Market(
                spot: req.Spot,
                rate: req.Rate,
                vol: req.Vol,
                dividendYield: req.DividendYield
            );

            var pricer = PricerFactory.Create(ConvertPricingMethod(req.PricingMethod));

            return pricer.Price(opt, market);
        }

        private PricingEngine.Pricing.PricingMethod ConvertPricingMethod(Shared.Enums.PricingMethod m)
        {
            return m switch
            {
                Shared.Enums.PricingMethod.BlackScholes => PricingEngine.Pricing.PricingMethod.BlackScholes,
                Shared.Enums.PricingMethod.Binomial => PricingEngine.Pricing.PricingMethod.Binomial,
                Shared.Enums.PricingMethod.MonteCarlo => PricingEngine.Pricing.PricingMethod.MonteCarlo,
                Shared.Enums.PricingMethod.LSMMonteCarlo => PricingEngine.Pricing.PricingMethod.LSMMonteCarlo,
                _ => throw new Exception("Unknown pricing method")
            };
        }
    }
}
