using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using static System.Math;

namespace PricingEngine.Tests
{
    public class MonteCarloVsBSTests
    {
        private const double TOL_MC = 0.05; 

        // ================================================================
        // 1. MC vs BS — Call, q = 0
        // ================================================================

        [Fact]
        public void MonteCarlo_Call_NoDividend_Matches_BS()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double mc = MonteCarloPricer.Price(
                opt, 
                mkt, 
                paths: 300_000, 
                antithetic: true
            );

            Assert.True(Abs(mc - bs) < TOL_MC);
        }

        // ================================================================
        // 2. MC vs BS — Call, q = 0.03
        // ================================================================

        [Fact]
        public void MonteCarlo_Call_WithDividend_Matches_BS()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double mc = MonteCarloPricer.Price(
                opt,
                mkt,
                paths: 300_000,
                antithetic: true
            );

            Assert.True(Abs(mc - bs) < TOL_MC);
        }

        // ================================================================
        // 3. MC convergence test — error decreases as 1/sqrt(N)
        // ================================================================

        [Fact]
        public void MonteCarlo_Convergence_SqrtN()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Put);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            double bs = BlackScholesPricer.Price(opt, mkt);

            double mc_20k  = MonteCarloPricer.Price(opt, mkt, paths: 10_000);
            double mc_80k  = MonteCarloPricer.Price(opt, mkt, paths: 80_000);
            double mc_320k = MonteCarloPricer.Price(opt, mkt, paths: 320_000);

            double err_20k  = Abs(mc_20k  - bs);
            double err_80k  = Abs(mc_80k  - bs);
            double err_320k = Abs(mc_320k - bs);

            Assert.True(err_80k  < err_20k);
            Assert.True(err_320k < err_80k);
        }

        [Fact]
        public void MonteCarlo_Antithetic_Has_Lower_Variance()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.0);

            int runs = 20;
            double sumVarStandard = 0;
            double sumVarAnti = 0;


            for (int i = 0; i < runs; i++)
            {
                double mc1 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: false);
                double mc2 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: false);
                double mc3 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: false);

                double meanStd = (mc1 + mc2 + mc3) / 3.0;
                sumVarStandard +=
                    (Pow(mc1 - meanStd, 2) +
                     Pow(mc2 - meanStd, 2) +
                     Pow(mc3 - meanStd, 2)) / 3.0;

                double a1 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: true);
                double a2 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: true);
                double a3 = MonteCarloPricer.Price(opt, mkt, 50_000, antithetic: true);

                double meanAnti = (a1 + a2 + a3) / 3.0;

                sumVarAnti +=
                    (Pow(a1 - meanAnti, 2) +
                     Pow(a2 - meanAnti, 2) +
                     Pow(a3 - meanAnti, 2)) / 3.0;
            }

            double avgVarStd  = sumVarStandard / runs;
            double avgVarAnti = sumVarAnti / runs;

            Assert.True(avgVarAnti < avgVarStd);
        }
    }
}
