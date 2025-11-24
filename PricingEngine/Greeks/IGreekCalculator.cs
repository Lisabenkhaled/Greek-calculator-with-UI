using PricingEngine.Models;

namespace PricingEngine.Greeks
{
    public interface IGreekCalculator
    {
        GreekResult Compute(Option option, Market market);
    }
}
