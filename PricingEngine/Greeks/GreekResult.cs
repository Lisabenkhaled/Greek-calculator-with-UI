namespace PricingEngine.Greeks
{
    public struct GreekResult
    {
        public double Delta { get; init; }
        public double Gamma { get; init; }
        public double Vega  { get; init; }
        public double Theta { get; init; }
        public double Rho   { get; init; }
        public double Vanna { get; init; }
        public double Vomma { get; init; }
        public double Zomma { get; init; }
    }
}
