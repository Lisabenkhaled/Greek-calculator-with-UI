using PricingEngine.Engines;
using PricingEngine.Models;
using PricingEngine.Greeks;
using Shared.DTO;
using Shared.Enums;

namespace Server.Services
{
    public class GreeksService
    {
        private readonly OptionFactory _factory;

        public GreeksService(OptionFactory factory)
        {
            _factory = factory;
        }

        public GreeksResponse ComputeGreeks(GreeksRequest req)
        {
            // Build option
            var opt = _factory.CreateOptionFromGreeks(req);

            // Build market
            var market = new Market(
                spot: req.Spot,
                rate: req.Rate,
                vol: req.Volatility,
                dividendYield: req.DividendYield
            );

            var greeks = GreekEngine.Compute(
                option: opt,
                market: market,
                greekMethod: ConvertGreekMethod(req.GreekMethod),
                pricingMethod: ConvertPricingMethod(req.PricingMethod)
            );

            return new GreeksResponse
            {
                Price = PriceEngine.Price(opt, market, ConvertPricingMethod(req.PricingMethod)),
                Delta = greeks.Delta,
                Gamma = greeks.Gamma,
                Vega  = greeks.Vega,
                Theta = greeks.Theta,
                Rho   = greeks.Rho,
                Vanna = greeks.Vanna,
                Vomma = greeks.Vomma,
                Zomma = greeks.Zomma
            };
        }

        private PricingEngine.Greeks.GreekMethod ConvertGreekMethod(Shared.Enums.GreekMethod m)
        {
            return m == Shared.Enums.GreekMethod.Analytic
                ? PricingEngine.Greeks.GreekMethod.Analytic
                : PricingEngine.Greeks.GreekMethod.FiniteDifference;
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
