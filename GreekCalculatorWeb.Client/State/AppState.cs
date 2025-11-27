using Shared.DTO;

namespace GreekCalculatorWeb.Client.State
{
    public class AppState
    {
        // On sauvegarde simplement la dernière requête de pricing
        public PricingRequest? LastPricing { get; set; }
    }
}
