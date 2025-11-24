using Shared.Enums;

namespace Shared.DTO
{
    public class PricingRequest
    {
        public string Ticker { get; set; } = "";
        public double Spot { get; set; }
        public double Strike { get; set; }
        public double Vol { get; set; }
        public double Rate { get; set; }
        public double DividendYield { get; set; } = 0;
        public double Maturity { get; set; }

        public OptionType OptionType { get; set; }
        public OptionStyle OptionStyle { get; set; }
        public PricingMethod PricingMethod { get; set; }
    }
}
