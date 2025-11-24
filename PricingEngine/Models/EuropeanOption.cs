namespace PricingEngine.Models
{
    public class EuropeanOption : Option
    {
        public EuropeanOption(double strike, double maturity, OptionType type)
            : base(strike, maturity, type, OptionStyle.European)
        {
        }

        public override Option Clone()
        {
            return new EuropeanOption(Strike, Maturity, Type);
        }
    }
}
