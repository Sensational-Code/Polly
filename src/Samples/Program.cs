#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.DependencyInjection;
using Polly.Registry;
using Polly.Timeout;
using Samples;

// Building directly
var strategy = new ResilienceStrategyBuilder()
    .AddTimeout(TimeSpan.FromSeconds(1))
    .Build();

await ExecuteStrategy(strategy, false);

// Building with IServiceCollection (simple)
strategy = new ServiceCollection()
    .AddResilienceStrategy("my-timeout", context => context.Builder.AddTimeout(TimeSpan.FromSeconds(1)))
    .BuildServiceProvider()
    .GetRequiredService<ResilienceStrategyProvider<string>>()
    .Get("my-timeout");

await ExecuteStrategy(strategy, false);

// Building with IServiceCollection (advanced)
strategy = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
    .Configure<TimeoutStrategyOptions>("dynamic-timeout-options", options =>
    {
        options.Timeout = TimeSpan.FromSeconds(1);
        options.OnTimeout
            .Register(() => Console.WriteLine("Oh, timeout occurred."))
            .Register(args => Console.WriteLine($"Oh, Timeout occurred after {args.Timeout}."))
            .Register(async args =>
            {
                Console.WriteLine($"Oh, Timeout occurred after {args.Timeout} and I am doing async work...");
                await Task.Delay(1000);
                Console.WriteLine($"Oh, Timeout occurred after {args.Timeout} and I am doing async work...finished");
            });
        options.TimeoutGenerator.SetGenerator(args =>
        {
            return args.Context.Properties.GetValue(ResilienceKeys.LongOperation, false) ?
                TimeSpan.FromSeconds(2) :
                TimeSpan.Zero;
        });
    })
    .AddResilienceStrategy("my-timeout", context =>
    {
        // resolve options from DI
        var options = context.ServiceProvider
                .GetRequiredService<IOptionsMonitor<TimeoutStrategyOptions>>()
                .Get("dynamic-timeout-options");

        // add a timeout strategy
        context.Builder.AddTimeout(options);
    })
    .BuildServiceProvider()
    .GetRequiredService<ResilienceStrategyProvider<string>>()
    .Get("my-timeout");

await ExecuteStrategy(strategy, true);
await ExecuteStrategy(strategy, false);

static async Task ExecuteStrategy(ResilienceStrategy strategy, bool longOperation)
{
    var watch = Stopwatch.StartNew();
    Console.WriteLine("Executing method...");

    try
    {
        var resilienceContext = ResilienceContext.Get();
        resilienceContext.Properties.Set(ResilienceKeys.LongOperation, longOperation);

        await strategy.ExecuteAsync(
            async (context, _) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);
            },
            resilienceContext,
            longOperation);
    }
    catch (TimeoutRejectedException e)
    {
        Console.WriteLine(e.Message);
    }

    Console.WriteLine($"Executing method...{watch.Elapsed.TotalMilliseconds}ms");
    Console.WriteLine();
}

