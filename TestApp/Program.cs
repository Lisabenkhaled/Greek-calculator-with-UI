using System;
using System.Threading.Tasks;
using PricingEngine.Models;
using PricingEngine.Pricing;
using PricingEngine.Greeks;
using PricingEngine.Engines;
using PricingEngine.Data;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("====================================================");
            Console.WriteLine("         LIVE MARKET GREEK CALCULATOR");
            Console.WriteLine("====================================================\n");

            var fetcher = new DataFetcher("FL1W5JO6O1R8O5MA");

            Console.Write("➡ Entre un ticker (ex: AAPL, TSLA, MSFT…) : ");
            string ticker = Console.ReadLine().Trim().ToUpper();

            Console.WriteLine("\n⏳ Récupération du spot…");
            var spot = await fetcher.GetSpotAsync(ticker);

            if (spot == null)
            {
                Console.WriteLine(" Impossible de récupérer le prix.");
                return;
            }

            Console.WriteLine("⏳ Récupération du taux sans risque…");
            var rf = await fetcher.GetRiskFreeRateAsync();

            if (rf == null)
            {
                Console.WriteLine(" Impossible de récupérer le taux sans risque.");
                return;
            }

            var cache = new DataCache
            {
                Spot = spot.Value,
                RiskFreeRate = rf.Value,
                Timestamp = DateTime.Now
            };

            cache.Save();
            Console.WriteLine("\n✔ Données sauvegardées dans le cache.\n");

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Ticker Spot = {cache.Spot:F2}");
            Console.WriteLine($"Risk-free rate = {cache.RiskFreeRate:P3}");
            Console.WriteLine("----------------------------------------------------\n");

            Console.WriteLine("====================================================");
            Console.WriteLine("         LIVE MARKET — FULL PRICING & GREEKS");
            Console.WriteLine("====================================================\n");

            double strike = 250.0;
            double maturity = 1.0;
            double vol = 0.20;
            double q = 0.00;

            var optCall = new EuropeanOption(strike, maturity, OptionType.Call);
            var optPut  = new EuropeanOption(strike, maturity, OptionType.Put);

            var mktLive = new Market(cache.Spot, cache.RiskFreeRate, vol, q);

            Console.WriteLine($"Spot (LIVE) = {cache.Spot:F4}");
            Console.WriteLine($"Risk-free (LIVE) = {cache.RiskFreeRate * 100:F4} %");
            Console.WriteLine($"Vol = {vol * 100:F2} %, Dividend = {q * 100:F2} %");
            Console.WriteLine("----------------------------------------------------\n");

            // EUROPEAN LIVE
            Console.WriteLine("************** LIVE EUROPEAN PRICES **************");
            PrintPriceTableEuropean(optCall, optPut, mktLive);

            Console.WriteLine("\n************** LIVE EUROPEAN GREEKS – CALL **************");
            PrintGreekTableEuropean(optCall, mktLive);

            Console.WriteLine("\n************** LIVE EUROPEAN GREEKS – PUT **************");
            PrintGreekTableEuropean(optPut, mktLive);

            // AMERICAN LIVE
            var optCallAm = new AmericanOption(strike, maturity, OptionType.Call);
            var optPutAm  = new AmericanOption(strike, maturity, OptionType.Put);

            Console.WriteLine("\n************** LIVE AMERICAN PRICES **************");
            PrintPriceTableAmerican(optCallAm, optPutAm, mktLive);

            Console.WriteLine("\n************** LIVE AMERICAN GREEKS – CALL **************");
            PrintGreekTableAmerican(optCallAm, mktLive);

            Console.WriteLine("\n************** LIVE AMERICAN GREEKS – PUT **************");
            PrintGreekTableAmerican(optPutAm, mktLive);

            Console.WriteLine("\n=== END LIVE PART ===\n\n");


            Console.WriteLine("====================================================");
            Console.WriteLine("           GREEK CALCULATOR – TEST STATICS");
            Console.WriteLine("   EU / AM – BS vs Binomial vs Monte Carlo + LSM (AM)");
            Console.WriteLine("====================================================\n");

            double strikeTest = 100.0;
            double maturityTest = 1.0;
            double rateTest = 0.05;
            double volTest = 0.20;

            var mktNoDiv = new Market(100.0, rateTest, volTest, 0.0);
            var mktDiv   = new Market(100.0, rateTest, volTest, 0.03);

            var callEU = new EuropeanOption(strikeTest, maturityTest, OptionType.Call);
            var putEU  = new EuropeanOption(strikeTest, maturityTest, OptionType.Put);

            var callAM = new AmericanOption(strikeTest, maturityTest, OptionType.Call);
            var putAM  = new AmericanOption(strikeTest, maturityTest, OptionType.Put);

            // EUROPEAN PRICES
            Console.WriteLine("************** EUROPEAN PRICES (q = 0.00) **************");
            PrintPriceTableEuropean(callEU, putEU, mktNoDiv);

            Console.WriteLine("\n************** EUROPEAN PRICES (q = 0.03) **************");
            PrintPriceTableEuropean(callEU, putEU, mktDiv);

            // EUROPEAN GREEKS
            Console.WriteLine("\n************** EUROPEAN GREEKS – CALL (q = 0.00) **************");
            PrintGreekTableEuropean(callEU, mktNoDiv);

            Console.WriteLine("\n************** EUROPEAN GREEKS – CALL (q = 0.03) **************");
            PrintGreekTableEuropean(callEU, mktDiv);

            Console.WriteLine("\n************** EUROPEAN GREEKS – PUT (q = 0.00) **************");
            PrintGreekTableEuropean(putEU, mktNoDiv);

            Console.WriteLine("\n************** EUROPEAN GREEKS – PUT (q = 0.03) **************");
            PrintGreekTableEuropean(putEU, mktDiv);

            // AMERICAN PRICES
            Console.WriteLine("\n====================================================");
            Console.WriteLine("                 AMERICAN OPTIONS");
            Console.WriteLine("====================================================");

            Console.WriteLine("\n************** AMERICAN PRICES (q = 0.00) **************");
            PrintPriceTableAmerican(callAM, putAM, mktNoDiv);

            Console.WriteLine("\n************** AMERICAN PRICES (q = 0.03) **************");
            PrintPriceTableAmerican(callAM, putAM, mktDiv);

            // AMERICAN GREEKS
            Console.WriteLine("\n************** AMERICAN GREEKS – CALL (q = 0.00) **************");
            PrintGreekTableAmerican(callAM, mktNoDiv);

            Console.WriteLine("\n************** AMERICAN GREEKS – CALL (q = 0.03) **************");
            PrintGreekTableAmerican(callAM, mktDiv);

            Console.WriteLine("\n************** AMERICAN GREEKS – PUT (q = 0.00) **************");
            PrintGreekTableAmerican(putAM, mktNoDiv);

            Console.WriteLine("\n************** AMERICAN GREEKS – PUT (q = 0.03) **************");
            PrintGreekTableAmerican(putAM, mktDiv);

            Console.WriteLine("\n=== END OF TESTS ===");
        }
        private static void PrintPriceTableEuropean(EuropeanOption call, EuropeanOption put, Market mkt)
        {
            Console.WriteLine($"Spot={mkt.Spot}, K={call.Strike}, T={call.Maturity}, r={mkt.Rate}, q={mkt.DividendYield}, σ={mkt.Vol}\n");

            Console.WriteLine("{0,-15} {1,15} {2,15}", "Model", "Call", "Put");
            Console.WriteLine(new string('-', 50));

            double callBS = PriceEngine.Price(call, mkt, PricingMethod.BlackScholes);
            double putBS  = PriceEngine.Price(put,  mkt, PricingMethod.BlackScholes);
            Console.WriteLine("{0,-15} {1,15:F6} {2,15:F6}", "Black-Scholes", callBS, putBS);

            double callBin = PriceEngine.Price(call, mkt, PricingMethod.Binomial);
            double putBin  = PriceEngine.Price(put,  mkt, PricingMethod.Binomial);
            Console.WriteLine("{0,-15} {1,15:F6} {2,15:F6}", "Binomial", callBin, putBin);

            double callMC = PriceEngine.Price(call, mkt, PricingMethod.MonteCarlo);
            double putMC  = PriceEngine.Price(put,  mkt, PricingMethod.MonteCarlo);
            Console.WriteLine("{0,-15} {1,15:F6} {2,15:F6}", "Monte Carlo", callMC, putMC);
        }

        private static void PrintPriceTableAmerican(AmericanOption call, AmericanOption put, Market mkt)
        {
            Console.WriteLine($"Spot={mkt.Spot}, K={call.Strike}, T={call.Maturity}, r={mkt.Rate}, q={mkt.DividendYield}, σ={mkt.Vol}\n");

            Console.WriteLine("{0,-20} {1,15} {2,15}", "Model", "Call", "Put");
            Console.WriteLine(new string('-', 60));

            double callBin = PriceEngine.Price(call, mkt, PricingMethod.Binomial);
            double putBin  = PriceEngine.Price(put,  mkt, PricingMethod.Binomial);
            Console.WriteLine("{0,-20} {1,15:F6} {2,15:F6}", "Binomial", callBin, putBin);

            double callLSM = PriceEngine.Price(call, mkt, PricingMethod.LSMMonteCarlo);
            double putLSM  = PriceEngine.Price(put,  mkt, PricingMethod.LSMMonteCarlo);
            Console.WriteLine("{0,-20} {1,15:F6} {2,15:F6}", "Monte Carlo LSM", callLSM, putLSM);
        }

        private static void PrintGreekTableEuropean(EuropeanOption opt, Market mkt)
        {
            Console.WriteLine($"Option={opt.Type}, Style=European, Spot={mkt.Spot}, K={opt.Strike}, T={opt.Maturity}, r={mkt.Rate}, q={mkt.DividendYield}, σ={mkt.Vol}\n");

            PrintGreekHeader();

            var gAnalytic = GreekEngine.Compute(opt, mkt, GreekMethod.Analytic, PricingMethod.BlackScholes);
            PrintGreekRow("Analytic (BS)", gAnalytic);

            var gFDBS = GreekEngine.Compute(opt, mkt, GreekMethod.FiniteDifference, PricingMethod.BlackScholes);
            PrintGreekRow("FD + BS", gFDBS);

            var gFDBin = GreekEngine.Compute(opt, mkt, GreekMethod.FiniteDifference, PricingMethod.Binomial);
            PrintGreekRow("FD + Binomial", gFDBin);

            var gFDMC = GreekEngine.Compute(opt, mkt, GreekMethod.FiniteDifference, PricingMethod.MonteCarlo);
            PrintGreekRow("FD + Monte Carlo", gFDMC);
        }

        private static void PrintGreekTableAmerican(AmericanOption opt, Market mkt)
        {
            Console.WriteLine($"Option={opt.Type}, Style=American, Spot={mkt.Spot}, K={opt.Strike}, T={opt.Maturity}, r={mkt.Rate}, q={mkt.DividendYield}, σ={mkt.Vol}\n");

            PrintGreekHeader();

            var gFDBin = GreekEngine.Compute(opt, mkt, GreekMethod.FiniteDifference, PricingMethod.Binomial);
            PrintGreekRow("FD + Binomial", gFDBin);

            var gFDLSM = GreekEngine.Compute(opt, mkt, GreekMethod.FiniteDifference, PricingMethod.LSMMonteCarlo);
            PrintGreekRow("FD + LSM", gFDLSM);
        }

        private static void PrintGreekHeader()
        {
            Console.WriteLine("{0,-22} {1,12} {2,12} {3,12} {4,12} {5,12} {6,12} {7,12} {8,12}",
                "Method", "Delta", "Gamma", "Vega", "Theta", "Rho",
                "Vanna", "Vomma", "Zomma");
            Console.WriteLine(new string('-', 120));
        }

        private static void PrintGreekRow(string label, GreekResult g)
        {
            Console.WriteLine(
                "{0,-22} {1,12:F6} {2,12:F6} {3,12:F6} {4,12:F6} {5,12:F6} {6,12:F6} {7,12:F6} {8,12:F6}",
                label, g.Delta, g.Gamma, g.Vega, g.Theta, g.Rho,
                g.Vanna, g.Vomma, g.Zomma
            );
        }
    }
}
