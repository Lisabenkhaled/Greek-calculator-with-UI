using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GreekCalculatorWeb.Client;
using GreekCalculatorWeb.Client.State;



namespace GreekCalculatorWeb.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5116")
            });
            builder.Services.AddSingleton<AppState>();
            
            await builder.Build().RunAsync();
        }
    }
}
