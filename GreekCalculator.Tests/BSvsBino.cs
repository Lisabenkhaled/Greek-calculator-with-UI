using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using static System.Math;

namespace PricingEngine.Tests
{
    public class BinomialVsBS_HighStepsTests
    {
        private const double TOL = 0.0005; 

        private const int N1 = 500;
        private const int N2 = 1000;
        private const int N3 = 2000;
        private const int N4 = 5000;

        // ================================================================
        // CALL — q = 0 — High precision test
        // ================================================================
        [Fact]
        public void Binomial_Call_NoDividend_HighSteps()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double b1 = BinomialTreePricer.PriceEuropean(opt, mkt, N1);
            double b2 = BinomialTreePricer.PriceEuropean(opt, mkt, N2);
            double b3 = BinomialTreePricer.PriceEuropean(opt, mkt, N3);
            double b4 = BinomialTreePricer.PriceEuropean(opt, mkt, N4);

            Assert.True(Abs(b1 - bs) < 0.015);
            Assert.True(Abs(b2 - bs) < 0.01);
            Assert.True(Abs(b3 - bs) < 0.007);
            Assert.True(Abs(b4 - bs) < TOL);
        }

        // ================================================================
        // PUT — q = 0 — High precision test
        // ================================================================
        [Fact]
        public void Binomial_Put_NoDividend_HighSteps()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double b1 = BinomialTreePricer.PriceEuropean(opt, mkt, N1);
            double b2 = BinomialTreePricer.PriceEuropean(opt, mkt, N2);
            double b3 = BinomialTreePricer.PriceEuropean(opt, mkt, N3);
            double b4 = BinomialTreePricer.PriceEuropean(opt, mkt, N4);

            Assert.True(Abs(b1 - bs) < 0.02);
            Assert.True(Abs(b2 - bs) < 0.002);
            Assert.True(Abs(b3 - bs) < 0.001);
            Assert.True(Abs(b4 - bs) < TOL);
        }

        // ================================================================
        // CALL — q = 3% dividend yield — High precision test
        // ================================================================
        [Fact]
        public void Binomial_Call_WithDividend_HighSteps()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double b1 = BinomialTreePricer.PriceEuropean(opt, mkt, N1);
            double b2 = BinomialTreePricer.PriceEuropean(opt, mkt, N2);
            double b3 = BinomialTreePricer.PriceEuropean(opt, mkt, N3);
            double b4 = BinomialTreePricer.PriceEuropean(opt, mkt, N4);

            Assert.True(Abs(b1 - bs) < 0.02);
            Assert.True(Abs(b2 - bs) < 0.002);
            Assert.True(Abs(b3 - bs) < 0.001);
            Assert.True(Abs(b4 - bs) < TOL);
        }

        // ================================================================
        // PUT — q = 3% dividend yield — High precision test
        // ================================================================
        [Fact]
        public void Binomial_Put_WithDividend_HighSteps()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double b1 = BinomialTreePricer.PriceEuropean(opt, mkt, N1);
            double b2 = BinomialTreePricer.PriceEuropean(opt, mkt, N2);
            double b3 = BinomialTreePricer.PriceEuropean(opt, mkt, N3);
            double b4 = BinomialTreePricer.PriceEuropean(opt, mkt, N4);

            Assert.True(Abs(b1 - bs) < 0.02);
            Assert.True(Abs(b2 - bs) < 0.002);
            Assert.True(Abs(b3 - bs) < 0.001);
            Assert.True(Abs(b4 - bs) < TOL);
        }
    }
}
