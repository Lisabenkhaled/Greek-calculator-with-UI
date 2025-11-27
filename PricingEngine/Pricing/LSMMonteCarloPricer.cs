using System;
using System.Collections.Generic;
using PricingEngine.Models;
using static System.Math;

namespace PricingEngine.Pricing
{
    public class LSMMonteCarloPricer : IOptionPricer
    {
        private readonly int _paths;
        private readonly int _steps;

        public LSMMonteCarloPricer(int paths = 200_000, int steps = 50)
        {
            _paths = paths;
            _steps = steps;
        }

        public double Price(Option opt, Market mkt)
        {
            if (opt is not AmericanOption amOpt)
                throw new ArgumentException("LSMMonteCarloPricer only supports AmericanOption.");

            return PriceAmericanLSM(amOpt, mkt);
        }
        private double PriceAmericanLSM(AmericanOption opt, Market mkt)
        {
            int N = _steps;
            int M = _paths;

            double dt   = opt.Maturity / N;
            double r    = mkt.Rate;
            double q    = mkt.DividendYield;
            double vol  = mkt.Vol;
            double S0   = mkt.Spot;

            double disc = Exp(-r * dt);
            double drift = (r - q - 0.5 * vol * vol) * dt;
            double diff  = vol * Sqrt(dt);

            double[,] S = new double[M, N + 1];
            var rng = new Random();

            for (int i = 0; i < M; i++)
            {
                S[i, 0] = S0;

                for (int t = 1; t <= N; t++)
                {
                    double z = NextGaussian(rng);
                    S[i, t] = S[i, t - 1] * Exp(drift + diff * z);
                }
            }

            double[] cashflow = new double[M];
            for (int i = 0; i < M; i++)
                cashflow[i] = Intrinsic(opt, S[i, N]);

            for (int t = N - 1; t >= 1; t--)
            {
                List<int> itm = new();
                for (int i = 0; i < M; i++)
                {
                    if (Intrinsic(opt, S[i, t]) > 0.0)
                        itm.Add(i);
                }

                if (itm.Count > 0)
                {
                    double[] X = new double[itm.Count];
                    double[] Y = new double[itm.Count];

                    for (int k = 0; k < itm.Count; k++)
                    {
                        int i = itm[k];
                        X[k] = S[i, t];
                        Y[k] = cashflow[i] * disc;
                    }

                    (double a, double b, double c) = PolynomialRegression(X, Y);

                    for (int k = 0; k < itm.Count; k++)
                    {
                        int i = itm[k];

                        double St = X[k];
                        double cont = a + b * St + c * St * St;
                        double exercise = Intrinsic(opt, St);

                        if (exercise > cont)
                            cashflow[i] = exercise; 
                        else
                            cashflow[i] = cashflow[i] * disc;
                    }
                }
                else
                {
                    for (int i = 0; i < M; i++)
                        cashflow[i] *= disc;
                }
            }

            double sum = 0.0;
            for (int i = 0; i < M; i++)
                sum += cashflow[i] * disc;

            return sum / M;
        }
        private static double Intrinsic(Option opt, double S)
        {
            double K = opt.Strike;
            return opt.Type == OptionType.Call
                ? Max(S - K, 0.0)
                : Max(K - S, 0.0);
        }

        private static double NextGaussian(Random rng)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            return Sqrt(-2.0 * Log(u1)) * Cos(2.0 * PI * u2);
        }
        private static (double a, double b, double c) PolynomialRegression(double[] X, double[] Y)
        {
            int n = X.Length;

            double sumX = 0, sumX2 = 0, sumX3 = 0, sumX4 = 0;
            double sumY = 0, sumXY = 0, sumX2Y = 0;

            for (int i = 0; i < n; i++)
            {
                double x = X[i];
                double x2 = x * x;

                sumX   += x;
                sumX2  += x2;
                sumX3  += x2 * x;
                sumX4  += x2 * x2;

                sumY   += Y[i];
                sumXY  += x * Y[i];
                sumX2Y += x2 * Y[i];
            }

            double[,] A =
            {
                { n,     sumX,  sumX2 },
                { sumX,  sumX2, sumX3 },
                { sumX2, sumX3, sumX4 }
            };

            double[] B = { sumY, sumXY, sumX2Y };

            double[] sol = Solve3x3(A, B);
            return (sol[0], sol[1], sol[2]);
        }

        private static double[] Solve3x3(double[,] A, double[] B)
        {
            double det =
                A[0,0] * (A[1,1]*A[2,2] - A[1,2]*A[2,1]) -
                A[0,1] * (A[1,0]*A[2,2] - A[1,2]*A[2,0]) +
                A[0,2] * (A[1,0]*A[2,1] - A[1,1]*A[2,0]);

            double invDet = 1.0 / det;

            double[,] adj = new double[3,3];

            adj[0,0] =  (A[1,1]*A[2,2] - A[1,2]*A[2,1]);
            adj[0,1] = -(A[0,1]*A[2,2] - A[0,2]*A[2,1]);
            adj[0,2] =  (A[0,1]*A[1,2] - A[0,2]*A[1,1]);

            adj[1,0] = -(A[1,0]*A[2,2] - A[1,2]*A[2,0]);
            adj[1,1] =  (A[0,0]*A[2,2] - A[0,2]*A[2,0]);
            adj[1,2] = -(A[0,0]*A[1,2] - A[0,2]*A[1,0]);

            adj[2,0] =  (A[1,0]*A[2,1] - A[1,1]*A[2,0]);
            adj[2,1] = -(A[0,0]*A[2,1] - A[0,1]*A[2,0]);
            adj[2,2] =  (A[0,0]*A[1,1] - A[0,1]*A[1,0]);

            double[] x = new double[3];
            for (int i = 0; i < 3; i++)
            {
                x[i] =
                    (adj[i,0] * B[0] +
                     adj[i,1] * B[1] +
                     adj[i,2] * B[2]) * invDet;
            }
            return x;
        }
    }
}
