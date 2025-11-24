using System;

namespace PricingEngine.Models.Payoffs
{
    public class CallPayoff : IPayoff
    {
        public double Strike { get; }

        public CallPayoff(double strike)
        {
            Strike = strike;
        }

        public double Evaluate(double spot)
        {
            return Math.Max(spot - Strike, 0.0);
        }
    }
}
