using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Add Services =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== Pricing / Greeks / Market services =====
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddSingleton<OptionFactory>();
builder.Services.AddSingleton<PricingService>();
builder.Services.AddSingleton<GreeksService>();

// ===== CORS for Blazor Client =====
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin(); // Dev only; change later if needed
    });
});

var app = builder.Build();

// ===== Middleware =====
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
