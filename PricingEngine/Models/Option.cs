using PricingEngine.Models.Payoffs;

namespace PricingEngine.Models
{
    public abstract class Option
    {
        public double Strike { get; set; }
        public double Maturity { get; set; }
        public OptionType Type { get; set; }
        public OptionStyle Style { get; set; }

        public IPayoff Payoff { get; protected set; }

        protected Option(double strike, double maturity, OptionType type, OptionStyle style)
        {
            Strike = strike;
            Maturity = maturity;
            Type = type;
            Style = style;

            Payoff = type switch
            {
                OptionType.Call => new CallPayoff(strike),
                OptionType.Put  => new PutPayoff(strike),
                _ => throw new NotSupportedException($"Unsupported OptionType {type}")
            };
        }

        public abstract Option Clone();
    }
}
