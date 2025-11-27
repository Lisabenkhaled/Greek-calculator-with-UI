namespace PricingEngine.Models
{
    public class TreeParameters
    {
        public int Steps { get; set; }
        public double UpFactor { get; set; }
        public double DownFactor { get; set; }
        public double RiskNeutralProb { get; set; }
        public double DiscountFactor { get; set; }
        public double Dt { get; set; }

        public TreeParameters(int steps)
        {
            Steps = steps;
        }
    }
}
