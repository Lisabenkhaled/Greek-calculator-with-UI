using PricingEngine.Models;
using PricingEngine.Models.Payoffs;
using PricingEngine.Utils;

namespace PricingEngine.Pricing
{
    public class BlackScholesPricer : IOptionPricer
    {
        public double Price(Option option, Market mkt)
        {
            return Price((EuropeanOption)option, mkt);
        }

        public static double Price(EuropeanOption opt, Market mkt)
        {
            double S = mkt.Spot;
            double K = opt.Strike;
            double r = mkt.Rate;
            double q = mkt.DividendYield;
            double vol = mkt.Vol;
            double T = opt.Maturity;

            double d1 = (Math.Log(S / K) + (r - q + 0.5 * vol * vol) * T) / (vol * Math.Sqrt(T));
            double d2 = d1 - vol * Math.Sqrt(T);

            if (opt.Type == OptionType.Call)
                return S * Math.Exp(-q * T) * MathUtil.NormCdf(d1) - K * Math.Exp(-r * T) * MathUtil.NormCdf(d2);

            else
                return K * Math.Exp(-r * T) * MathUtil.NormCdf(-d2) - S * Math.Exp(-q * T) * MathUtil.NormCdf(-d1);
        }
    }
}
