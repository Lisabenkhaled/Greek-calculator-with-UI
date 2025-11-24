using Shared.Enums;

namespace Shared.DTO
{
    public class GreeksRequest
    {
        public string Ticker { get; set; } = string.Empty;

        public double Spot { get; set; }

        public double Strike { get; set; }

        public double Rate { get; set; }   // Taux sans risque

        public double Maturity { get; set; } // en années

        public double Volatility { get; set; }

        public double DividendYield { get; set; }

        public OptionType OptionType { get; set; }

        public OptionStyle OptionStyle { get; set; }

        public GreekMethod GreekMethod { get; set; }

        public PricingMethod PricingMethod { get; set; }
    }
}
