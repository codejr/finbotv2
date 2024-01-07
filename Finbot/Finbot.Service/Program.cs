
using Discord.Interactions;
using Discord.WebSocket;
using Finbot.Data;
using Finbot.Discord;
using Finbot.MarketData;
using Finbot.Service;
using Finbot.Trading;
using Microsoft.EntityFrameworkCore;
using Sentry;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddHttpClient<IMarketDataClient, MarketDataClient>(client =>
{
    client.BaseAddress = new Uri(config.GetValue<string>("MarketData:BaseUri") ?? "");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("X-RapidAPI-Key", config.GetValue<string>("MarketData:ApiKey"));
    client.DefaultRequestHeaders.Add("X-RapidAPI-Host", config.GetValue<string>("MarketData:ApiHost"));
});

SentrySdk.Init(options =>
{
    options.Dsn = config.GetValue<string>("Sentry:Dsn");

    options.Debug = true;
    options.AutoSessionTracking = true;
    options.EnableTracing = true;
});

builder.Services.AddDbContext<FinbotDataContext>(options => options.UseSqlite(config.GetConnectionString("TradingDB")));

builder.Services.AddTransient<IPortfolioManager, PortfolioManager>();
builder.Services.AddTransient<ITradeManager, TradeManager>();
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<FinbotDiscordBot>();
builder.Services.AddHostedService<FinbotService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinbotDataContext>();
    db.Database.Migrate();
}

host.UseRouting();

host.MapGet("/", () => "");

await host.RunAsync();