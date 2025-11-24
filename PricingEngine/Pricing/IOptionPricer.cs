using PricingEngine.Models;

namespace PricingEngine.Pricing
{
    public interface IOptionPricer
    {
        double Price(Option option, Market market);
    }
}
