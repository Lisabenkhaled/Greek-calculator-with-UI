namespace PricingEngine.Models
{
    public class AmericanOption : Option
    {
        public AmericanOption(double strike, double maturity, OptionType type)
            : base(strike, maturity, type, OptionStyle.American)
        {
        }

        public override Option Clone()
        {
            return new AmericanOption(Strike, Maturity, Type);
        }
    }
}
