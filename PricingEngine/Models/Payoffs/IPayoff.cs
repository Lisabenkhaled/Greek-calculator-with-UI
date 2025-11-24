namespace PricingEngine.Models.Payoffs
{
    public interface IPayoff
    {
        double Evaluate(double spot);
    }
}
