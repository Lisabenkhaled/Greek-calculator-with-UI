using PricingEngine.Models;
using Shared.DTO;
using Shared.Enums;

namespace Server.Services
{
    public class OptionFactory
    {
        public Option CreateOptionFromPricing(PricingRequest req)
        {
            return req.OptionStyle switch
            {
                Shared.Enums.OptionStyle.European =>
                    new EuropeanOption(req.Strike, req.Maturity, ConvertOptionType(req.OptionType)),

                Shared.Enums.OptionStyle.American =>
                    new AmericanOption(req.Strike, req.Maturity, ConvertOptionType(req.OptionType)),

                _ => throw new Exception("Invalid option style")
            };
        }

        public Option CreateOptionFromGreeks(GreeksRequest req)
        {
            return req.OptionStyle switch
            {
                Shared.Enums.OptionStyle.European =>
                    new EuropeanOption(req.Strike, req.Maturity, ConvertOptionType(req.OptionType)),

                Shared.Enums.OptionStyle.American =>
                    new AmericanOption(req.Strike, req.Maturity, ConvertOptionType(req.OptionType)),

                _ => throw new Exception("Invalid option style")
            };
        }

        private PricingEngine.Models.OptionType ConvertOptionType(Shared.Enums.OptionType t)
        {
            return t switch
            {
                Shared.Enums.OptionType.Call => PricingEngine.Models.OptionType.Call,
                Shared.Enums.OptionType.Put => PricingEngine.Models.OptionType.Put,
                _ => throw new Exception("Invalid option type")
            };
        }
    }
}
