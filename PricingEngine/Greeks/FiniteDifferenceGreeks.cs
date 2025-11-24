using System;
using PricingEngine.Models;
using PricingEngine.Pricing;
using static System.Math;

namespace PricingEngine.Greeks
{
    /// <summary>
    /// Finite Difference Greeks calculator.
    /// Supports ANY pricing method (BS, Binomial, MC, etc.)
    /// </summary>
    public class FiniteDifferenceGreeks : IGreekCalculator
    {
        private readonly IOptionPricer _pricer;
        private readonly double _hS;
        private readonly double _hV;
        private readonly double _hT;
        private readonly double _hR;

        public FiniteDifferenceGreeks(
            IOptionPricer pricer,
            double hS = 0.5,
            double hV = 0.01,
            double hT = 0.2,
            double hR = 0.1)
        {
            _pricer = pricer;
            _hS = hS;
            _hV = hV;
            _hT = hT;
            _hR = hR;
        }

        public GreekResult Compute(Option option, Market mkt)
        {
            return ComputeGreeks(option, mkt);
        }

        // ================================================================
        //   CORE FINITE DIFFERENCE IMPLEMENTATION
        // ================================================================
        private GreekResult ComputeGreeks(Option opt, Market mkt)
        {
            double V0 = _pricer.Price(opt, mkt);

            // ---------- DELTA ----------
            var mSp = CloneMarket(mkt, spot: mkt.Spot + _hS);
            var mSm = CloneMarket(mkt, spot: mkt.Spot - _hS);

            double fSp = _pricer.Price(opt.Clone(), mSp);
            double fSm = _pricer.Price(opt.Clone(), mSm);

            double delta = (fSp - fSm) / (2 * _hS);

            // ---------- GAMMA (5-point stencil) ----------
            var mSp2 = CloneMarket(mkt, spot: mkt.Spot + 2 * _hS);
            var mSm2 = CloneMarket(mkt, spot: mkt.Spot - 2 * _hS);

            double fSp2 = _pricer.Price(opt.Clone(), mSp2);
            double fSm2 = _pricer.Price(opt.Clone(), mSm2);

            double gamma =
                (-fSp2 + 16 * fSp - 30 * V0 + 16 * fSm - fSm2) /
                (12 * _hS * _hS);

            // ---------- VEGA ----------
            var mVp  = CloneMarket(mkt, vol: mkt.Vol + _hV);
            var mVm  = CloneMarket(mkt, vol: mkt.Vol - _hV);
            var mVp2 = CloneMarket(mkt, vol: mkt.Vol + 2 * _hV);
            var mVm2 = CloneMarket(mkt, vol: mkt.Vol - 2 * _hV);

            double fVp  = _pricer.Price(opt.Clone(), mVp);
            double fVm  = _pricer.Price(opt.Clone(), mVm);
            double fVp2 = _pricer.Price(opt.Clone(), mVp2);
            double fVm2 = _pricer.Price(opt.Clone(), mVm2);

            double vega =
                (-fVp2 + 8 * fVp - 8 * fVm + fVm2) /
                (12 * _hV);

            // ---------- VOMMA (second derivative wrt Vol) ----------
            double vomma =
                (-fVp2 + 16 * fVp - 30 * V0 + 16 * fVm - fVm2) /
                (12 * _hV * _hV);

            // ---------- THETA ----------
            var oTp  = CloneOptionWithMaturity(opt, opt.Maturity + _hT);
            var oTm  = CloneOptionWithMaturity(opt, opt.Maturity - _hT);
            var oTp2 = CloneOptionWithMaturity(opt, opt.Maturity + 2 * _hT);
            var oTm2 = CloneOptionWithMaturity(opt, opt.Maturity - 2 * _hT);

            double fTp  = _pricer.Price(oTp,  mkt);
            double fTm  = _pricer.Price(oTm,  mkt);
            double fTp2 = _pricer.Price(oTp2, mkt);
            double fTm2 = _pricer.Price(oTm2, mkt);

            double thetaRaw =
                (-fTp2 + 8 * fTp - 8 * fTm + fTm2) /
                (12 * _hT);

            double theta = -thetaRaw; // market convention

            // ---------- RHO ----------
            var mRp  = CloneMarket(mkt, rate: mkt.Rate + _hR);
            var mRm  = CloneMarket(mkt, rate: mkt.Rate - _hR);
            var mRp2 = CloneMarket(mkt, rate: mkt.Rate + 2 * _hR);
            var mRm2 = CloneMarket(mkt, rate: mkt.Rate - 2 * _hR);

            double fRp  = _pricer.Price(opt.Clone(), mRp);
            double fRm  = _pricer.Price(opt.Clone(), mRm);
            double fRp2 = _pricer.Price(opt.Clone(), mRp2);
            double fRm2 = _pricer.Price(opt.Clone(), mRm2);

            double rho =
                (-fRp2 + 8 * fRp - 8 * fRm + fRm2) /
                (12 * _hR);

            // =====================================================
            //   SECOND ORDER GREEKS
            // =====================================================

            // ---------- VANNA (cross derivative S-Vol) ----------
            var mSp_Vp = CloneMarket(mkt, spot: mkt.Spot + _hS, vol: mkt.Vol + _hV);
            var mSm_Vp = CloneMarket(mkt, spot: mkt.Spot - _hS, vol: mkt.Vol + _hV);
            var mSp_Vm = CloneMarket(mkt, spot: mkt.Spot + _hS, vol: mkt.Vol - _hV);
            var mSm_Vm = CloneMarket(mkt, spot: mkt.Spot - _hS, vol: mkt.Vol - _hV);

            double fSp_Vp = _pricer.Price(opt.Clone(), mSp_Vp);
            double fSm_Vp = _pricer.Price(opt.Clone(), mSm_Vp);
            double fSp_Vm = _pricer.Price(opt.Clone(), mSp_Vm);
            double fSm_Vm = _pricer.Price(opt.Clone(), mSm_Vm);

            double vanna = (fSp_Vp - fSm_Vp - fSp_Vm + fSm_Vm) / (4 * _hS * _hV);

            // ---------- ZOMMA (derivative of Gamma wrt Vol) ----------
            double gamma_pV =
                (
                    -_pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot + 2 * _hS, vol: mkt.Vol + _hV))
                    + 16 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot + _hS, vol: mkt.Vol + _hV))
                    - 30 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot, vol: mkt.Vol + _hV))
                    + 16 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot - _hS, vol: mkt.Vol + _hV))
                    - _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot - 2 * _hS, vol: mkt.Vol + _hV))
                ) / (12 * _hS * _hS);

            double gamma_mV =
                (
                    -_pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot + 2 * _hS, vol: mkt.Vol - _hV))
                    + 16 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot + _hS, vol: mkt.Vol - _hV))
                    - 30 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot, vol: mkt.Vol - _hV))
                    + 16 * _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot - _hS, vol: mkt.Vol - _hV))
                    - _pricer.Price(opt.Clone(), CloneMarket(mkt, spot: mkt.Spot - 2 * _hS, vol: mkt.Vol - _hV))
                ) / (12 * _hS * _hS);

            double zomma = (gamma_pV - gamma_mV) / (2 * _hV);

            // =====================================================
            //   RETURN ALL GREEKS
            // =====================================================
            return new GreekResult
            {
                Delta = delta,
                Gamma = gamma,
                Vega  = vega,
                Theta = theta,
                Rho   = rho,

                Vanna = vanna,
                Vomma = vomma,
                Zomma = zomma
            };
        }

        // ================================================================
        //   HELPERS
        // ================================================================
        private static Market CloneMarket(Market m,
                                          double? spot = null,
                                          double? vol  = null,
                                          double? rate = null)
        {
            return new Market(
                spot ?? m.Spot,
                rate ?? m.Rate,
                vol  ?? m.Vol,
                m.DividendYield
            );
        }

        private static Option CloneOptionWithMaturity(Option o, double newMaturity)
        {
            var clone = o.Clone();
            clone.Maturity = newMaturity;
            return clone;
        }
    }
}
