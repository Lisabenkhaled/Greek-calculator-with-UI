using System;

namespace PricingEngine.Models.Payoffs
{
    public class PutPayoff : IPayoff
    {
        public double Strike { get; }

        public PutPayoff(double strike)
        {
            Strike = strike;
        }

        public double Evaluate(double spot)
        {
            return Math.Max(Strike - spot, 0.0);
        }
    }
}
