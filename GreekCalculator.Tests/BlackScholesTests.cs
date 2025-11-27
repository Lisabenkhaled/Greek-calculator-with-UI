using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using PricingEngine.Greeks;
using static System.Math;

namespace PricingEngine.Tests
{
    public class BlackScholesTests
    {
        private const double PRICE_TOL = 1e-4;
        private const double GREEK_TOL = 1e-4;

        [Fact]
        public void BS_Price_Call_NoDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double price = BlackScholesPricer.Price(opt, mkt);
            double expected = 10.4505835721856;

            Assert.True(Abs(price - expected) < PRICE_TOL,
                $"Expected {expected}, got {price}");
        }

        [Fact]
        public void BS_Price_Put_NoDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double price = BlackScholesPricer.Price(opt, mkt);
            double expected = 5.57352602225697;

            Assert.True(Abs(price - expected) < PRICE_TOL,
                $"Expected {expected}, got {price}");
        }

        [Fact]
        public void BS_Price_Call_WithDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            double price = BlackScholesPricer.Price(opt, mkt);
            double expected = 8.65252855394271;

            Assert.True(Abs(price - expected) < PRICE_TOL,
                $"Expected {expected}, got {price}");
        }

        [Fact]
        public void BS_Price_Put_WithDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            double price = BlackScholesPricer.Price(opt, mkt);
            double expected = 6.7309176491633;

            Assert.True(Abs(price - expected) < PRICE_TOL,
                $"Expected {expected}, got {price}");
        }

        [Fact]
        public void BS_Greeks_Call_NoDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            var g = AnalyticGreeks.Compute(opt, mkt);

            Assert.True(Abs(g.Delta - 0.636830651175619) < GREEK_TOL);
            Assert.True(Abs(g.Gamma - 0.0187620173458469) < GREEK_TOL);
            Assert.True(Abs(g.Vega  - 37.5240346916938) < GREEK_TOL);
            Assert.True(Abs(g.Theta - (-6.4140275464382)) < GREEK_TOL);
            Assert.True(Abs(g.Rho   - 53.2324815453763) < GREEK_TOL);
        }

        [Fact]
        public void BS_Greeks_Call_WithDividend()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            var g = AnalyticGreeks.Compute(opt, mkt);

            Assert.True(Abs(g.Delta - 0.562139997789784) < GREEK_TOL);
            Assert.True(Abs(g.Gamma - 0.0189742817897629) < GREEK_TOL);
            Assert.True(Abs(g.Vega  - 37.9485635795257) < GREEK_TOL);
            Assert.True(Abs(g.Theta - (-4.48650992583501)) < GREEK_TOL);
            Assert.True(Abs(g.Rho   - 47.5614712250357) < GREEK_TOL);
        }
    }
}
