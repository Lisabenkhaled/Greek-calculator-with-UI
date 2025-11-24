namespace PricingEngine.Greeks
{
    public struct GreekResult
    {
        // First-order Greeks
        public double Delta { get; init; }
        public double Gamma { get; init; }
        public double Vega  { get; init; }
        public double Theta { get; init; }
        public double Rho   { get; init; }

        // Second-order Greeks
        public double Vanna { get; init; }  // ∂²Price / ∂S∂σ
        public double Vomma { get; init; }  // ∂²Price / ∂σ²
        public double Zomma { get; init; }  // ∂³Price / ∂S²∂σ
    }
}
