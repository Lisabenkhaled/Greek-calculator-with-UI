namespace PricingEngine.Models
{
    /// <summary>
    /// Stores all parameters needed for building a binomial tree.
    /// Can be extended later for trinomial or LR/Cox-Ross-Rubinstein variants.
    /// </summary>
    public class TreeParameters
    {
        public int Steps { get; set; }                     // number of time steps
        public double UpFactor { get; set; }               // u
        public double DownFactor { get; set; }             // d
        public double RiskNeutralProb { get; set; }        // p
        public double DiscountFactor { get; set; }         // e^{-r dt}
        public double Dt { get; set; }                     // dt = T/Steps

        public TreeParameters(int steps)
        {
            Steps = steps;
        }
    }
}
