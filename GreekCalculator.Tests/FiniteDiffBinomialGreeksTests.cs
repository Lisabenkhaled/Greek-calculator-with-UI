using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using PricingEngine.Greeks;
using static System.Math;

namespace PricingEngine.Tests
{
    public class FiniteDiffBinomialGreeksTests
    {
        // Binomial tree steps for Greeks (relativement élevé)
        private const int STEPS = 1000;

        // Tolerances for FD + Binomial vs BS
        // (Binomial + FD => moins précis que BS + Analytique ou BS + FD)
        private const double TOL_DELTA = 1e-2;   
        private const double TOL_GAMMA = 5e-2;   
        private const double TOL_VEGA  = 1e-2;   
        private const double TOL_THETA = 1e-2;   
        private const double TOL_RHO   = 1e-2;   

        // ============================================================
        // 1) NO DIVIDEND (q = 0) — Compare:
        //    - BS + Analytic Greeks (reference)
        //    - BS + FD
        //    - Binomial + FD
        // ============================================================

        [Fact]
        public void FD_Binomial_Greeks_NoDividend_Close_To_BS()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            // 1) Référence : Black–Scholes + Analytical Greeks
            var gAnalytic = AnalyticGreeks.Compute(opt, mkt);

            // 2) BS + FD (hautement précis)
            var gFdBs = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BlackScholesPricer.Price((EuropeanOption)o, mk)
            );

            // 3) Binomial Tree + FD
            var gFdBin = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceEuropean((EuropeanOption)o, mk, STEPS)
            );

            // --- Sanity check : FD-BS ~ Analytic (devrait être très proche)
            Assert.True(Abs(gFdBs.Delta - gAnalytic.Delta) < 1e-4);
            Assert.True(Abs(gFdBs.Gamma - gAnalytic.Gamma) < 1e-3);
            Assert.True(Abs(gFdBs.Vega  - gAnalytic.Vega ) < 1e-3);
            Assert.True(Abs(gFdBs.Theta - gAnalytic.Theta) < 1e-2);
            Assert.True(Abs(gFdBs.Rho   - gAnalytic.Rho  ) < 1e-3);

            // --- Test principal : FD-Binomial vs Analytic BS
            Assert.True(Abs(gFdBin.Delta - gAnalytic.Delta) < TOL_DELTA);
            Assert.True(Abs(gFdBin.Gamma - gAnalytic.Gamma) < TOL_GAMMA);
            Assert.True(Abs(gFdBin.Vega  - gAnalytic.Vega ) < TOL_VEGA);
            Assert.True(Abs(gFdBin.Theta - gAnalytic.Theta) < TOL_THETA);
            Assert.True(Abs(gFdBin.Rho   - gAnalytic.Rho  ) < TOL_RHO);

            // --- Bonus : FD-Binomial vs FD-BS (deux approches numériques)
            Assert.True(Abs(gFdBin.Delta - gFdBs.Delta) < TOL_DELTA);
            Assert.True(Abs(gFdBin.Gamma - gFdBs.Gamma) < TOL_GAMMA);
            Assert.True(Abs(gFdBin.Vega  - gFdBs.Vega ) < TOL_VEGA);
            Assert.True(Abs(gFdBin.Theta - gFdBs.Theta) < TOL_THETA);
            Assert.True(Abs(gFdBin.Rho   - gFdBs.Rho  ) < TOL_RHO);
        }

        // ============================================================
        // 2) WITH DIVIDEND (q = 0.03) — même logique
        // ============================================================

        [Fact]
        public void FD_Binomial_Greeks_WithDividend_Close_To_BS()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            // 1) Référence : Black–Scholes + Analytical Greeks (avec q)
            var gAnalytic = AnalyticGreeks.Compute(opt, mkt);

            // 2) BS + FD
            var gFdBs = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BlackScholesPricer.Price((EuropeanOption)o, mk)
            );

            // 3) Binomial Tree + FD
            var gFdBin = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BinomialTreePricer.PriceEuropean((EuropeanOption)o, mk, STEPS)
            );

            // Sanity check : FD-BS ~ Analytic
            Assert.True(Abs(gFdBs.Delta - gAnalytic.Delta) < 1e-4);
            Assert.True(Abs(gFdBs.Gamma - gAnalytic.Gamma) < 1e-3);
            Assert.True(Abs(gFdBs.Vega  - gAnalytic.Vega ) < 1e-3);
            Assert.True(Abs(gFdBs.Theta - gAnalytic.Theta) < 1e-2);
            Assert.True(Abs(gFdBs.Rho   - gAnalytic.Rho  ) < 5e-3);

            // Test principal : FD-Binomial vs Analytic BS
            Assert.True(Abs(gFdBin.Delta - gAnalytic.Delta) < TOL_DELTA);
            Assert.True(Abs(gFdBin.Gamma - gAnalytic.Gamma) < TOL_GAMMA);
            Assert.True(Abs(gFdBin.Vega  - gAnalytic.Vega ) < TOL_VEGA);
            Assert.True(Abs(gFdBin.Theta - gAnalytic.Theta) < TOL_THETA);
            Assert.True(Abs(gFdBin.Rho   - gAnalytic.Rho  ) < TOL_RHO);

            // Bonus : FD-Binomial vs FD-BS
            Assert.True(Abs(gFdBin.Delta - gFdBs.Delta) < TOL_DELTA);
            Assert.True(Abs(gFdBin.Gamma - gFdBs.Gamma) < TOL_GAMMA);
            Assert.True(Abs(gFdBin.Vega  - gFdBs.Vega ) < TOL_VEGA);
            Assert.True(Abs(gFdBin.Theta - gFdBs.Theta) < TOL_THETA);
            Assert.True(Abs(gFdBin.Rho   - gFdBs.Rho  ) < TOL_RHO);
        }
    }
}
