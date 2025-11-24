namespace PricingEngine.Models;

public class Market
{
    public double Spot { get; set; }
    public double Rate { get; set; }
    public double Vol { get; set; }
    public double DividendYield { get; set; }

    public Market(double spot, double rate, double vol, double dividendYield = 0.0)
    {
        Spot = spot;
        Rate = rate;
        Vol = vol;
        DividendYield = dividendYield;
    }

    public Market Clone() => new(Spot, Rate, Vol, DividendYield);
}
