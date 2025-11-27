namespace Shared.DTO
{
    public class GreeksResponse
    {
        public double Price { get; set; }
        public double Delta { get; set; }
        public double Gamma { get; set; }
        public double Vega { get; set; }
        public double Theta { get; set; }
        public double Rho { get; set; }

        public double Vanna { get; set; }
        public double Vomma { get; set; }
        public double Zomma { get; set; }
    }
}
