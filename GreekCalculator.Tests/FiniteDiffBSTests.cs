using Xunit;
using PricingEngine.Models;
using PricingEngine.Pricing;
using PricingEngine.Greeks;
using static System.Math;

namespace PricingEngine.Tests
{
    public class FiniteDiffBSTests
    {
        [Fact]
        public void FD_Greeks_Match_Analytic_BS()
        {
            var opt = new EuropeanOption(100, 1.0, OptionType.Call);
            var mkt = new Market(100, 0.05, 0.20, 0.03);

            var gA  = AnalyticGreeks.Compute(opt, mkt);

            var gFD = FiniteDifferenceGreeks.Compute(
                opt,
                mkt,
                (o, mk) => BlackScholesPricer.Price((EuropeanOption)o, mk)
            );

            // Tolérances réalistes pour FD (4th order)
            const double TOL_DELTA = 1e-3;
            const double TOL_GAMMA = 1e-3;
            const double TOL_VEGA  = 1e-3;
            const double TOL_THETA = 5e-3;
            const double TOL_RHO   = 5e-3;

            Assert.True(Abs(gFD.Delta - gA.Delta) < TOL_DELTA, $"Delta FD={gFD.Delta}, Ana={gA.Delta}");
            Assert.True(Abs(gFD.Gamma - gA.Gamma) < TOL_GAMMA, $"Gamma FD={gFD.Gamma}, Ana={gA.Gamma}");
            Assert.True(Abs(gFD.Vega  - gA.Vega ) < TOL_VEGA,  $"Vega FD={gFD.Vega}, Ana={gA.Vega}");
            Assert.True(Abs(gFD.Theta - gA.Theta) < TOL_THETA, $"Theta FD={gFD.Theta}, Ana={gA.Theta}");
            Assert.True(Abs(gFD.Rho   - gA.Rho  ) < TOL_RHO,   $"Rho FD={gFD.Rho}, Ana={gA.Rho}");
        }
    }
}
