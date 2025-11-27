using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using PricingEngine.Greeks;
using static System.Math;

namespace PricingEngine.Tests
{
    public class AmericanGreeksFDTests
    {
        private const int N_FINE   = 2000; 
        private const int N_MEDIUM = 1000; 

        private const double TOL_DELTA = 0.02;
        private const double TOL_GAMMA = 0.02;
        private const double TOL_VEGA  = 0.02;
        private const double TOL_THETA = 0.02;
        private const double TOL_RHO   = 0.02;


        // AMERICAN PUT — NO DIVIDEND (q = 0)

        [Fact]
        public void AmericanPut_FD_Greeks_StableAcrossSteps_NoDividend()
        {
            var opt = new AmericanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.0);


            var g1 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_MEDIUM)
            );

            var g2 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_FINE)
            );

            Assert.InRange(g2.Delta, -1.0, 0.0);
            Assert.True(g2.Gamma >= 0);
            Assert.True(g2.Theta <= 0);
            Assert.True(g2.Vega >= 0);

            Assert.True(Abs(g1.Delta - g2.Delta) < TOL_DELTA);
            Assert.True(Abs(g1.Gamma - g2.Gamma) < TOL_GAMMA);
            Assert.True(Abs(g1.Vega  - g2.Vega ) < TOL_VEGA);
            Assert.True(Abs(g1.Theta - g2.Theta) < TOL_THETA);
            Assert.True(Abs(g1.Rho   - g2.Rho  ) < TOL_RHO);
        }


        // AMERICAN CALL — WITH DIVIDEND (q = 0.03)

        [Fact]
        public void AmericanCall_FD_Greeks_StableAcrossSteps_WithDividend()
        {
            var opt = new AmericanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            var g1 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_MEDIUM)
            );

            var g2 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_FINE)
            );

            Assert.InRange(g2.Delta, 0.0, 1.0);
            Assert.True(g2.Gamma >= 0);
            Assert.True(g2.Theta <= 0);
            Assert.True(g2.Vega >= 0);

            Assert.True(Abs(g1.Delta - g2.Delta) < TOL_DELTA);
            Assert.True(Abs(g1.Gamma - g2.Gamma) < TOL_GAMMA);
            Assert.True(Abs(g1.Vega  - g2.Vega ) < TOL_VEGA);
            Assert.True(Abs(g1.Theta - g2.Theta) < TOL_THETA);
            Assert.True(Abs(g1.Rho   - g2.Rho  ) < TOL_RHO);
        }

        // AMERICAN PUT — WITH DIVIDEND (q = 0.03)

        [Fact]
        public void AmericanPut_FD_Greeks_StableAcrossSteps_WithDividend()
        {
            var opt = new AmericanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            var g1 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_MEDIUM)
            );

            var g2 = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceAmerican((AmericanOption)o, mk, N_FINE)
            );

            Assert.InRange(g2.Delta, -1.0, 0.0);
            Assert.True(g2.Gamma >= 0);
            Assert.True(g2.Theta <= 0);
            Assert.True(g2.Vega >= 0);

            Assert.True(Abs(g1.Delta - g2.Delta) < TOL_DELTA);
            Assert.True(Abs(g1.Gamma - g2.Gamma) < TOL_GAMMA);
            Assert.True(Abs(g1.Vega  - g2.Vega ) < TOL_VEGA);
            Assert.True(Abs(g1.Theta - g2.Theta) < TOL_THETA);
            Assert.True(Abs(g1.Rho   - g2.Rho  ) < TOL_RHO);
        }
    }
}
