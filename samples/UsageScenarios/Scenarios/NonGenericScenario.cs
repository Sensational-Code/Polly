using Polly;
using UsageScenarios;

namespace UsageScenarios.Scenarios;

internal class NonGenericScenario : Scenario
{
    public override IApi WrapApi(IApi originalApi, bool onlyExceptions)
    {
        if (onlyExceptions)
        {
            var strategy = new ResilienceStrategyBuilder()
                .AddRetry(new()
                {
                    ShouldHandle = args => args.Exception switch
                    {
                        HttpRequestException => PredicateResult.True,
                        _ => PredicateResult.False,
                    }
                })
                .AddAdvancedCircuitBreaker(new()
                {
                    ShouldHandle = args => args.Exception switch
                    {
                        HttpRequestException => PredicateResult.True,
                        _ => PredicateResult.False,
                    }
                })
                .Build();

            return new ApiImpl(originalApi, strategy);
        }
        else
        {
            var strategy = new ResilienceStrategyBuilder()
                .AddRetry(new()
                {
                    ShouldHandle = args => args switch
                    {
                        { Exception: HttpRequestException } => PredicateResult.True,
                        { Result: ResultA { IsError: true } } => PredicateResult.True,
                        { Result: ResultB { IsError: true } } => PredicateResult.True,
                        { Result: ResultC { IsError: true } } => PredicateResult.True,
                        _ => PredicateResult.False,
                    }
                })
                .AddAdvancedCircuitBreaker(new()
                {
                    ShouldHandle = args => args switch
                    {
                        { Exception: HttpRequestException } => PredicateResult.True,
                        { Result: ResultA { IsError: true } } => PredicateResult.True,
                        { Result: ResultB { IsError: true } } => PredicateResult.True,
                        { Result: ResultC { IsError: true } } => PredicateResult.True,
                        _ => PredicateResult.False,
                    }
                })
                .Build();

            return new ApiImpl(originalApi, strategy);
        }
    }

    private class ApiImpl : IApi
    {
        private readonly IApi api;
        private readonly ResilienceStrategy resilienceStrategy;

        public ApiImpl(IApi api, ResilienceStrategy resilienceStrategy)
        {
            this.api = api;
            this.resilienceStrategy = resilienceStrategy;
        }

        public void CallA(ResultA result)
            => resilienceStrategy.Execute(static state => state.api.CallA(state.result), (api, result));

        public void CallB(ResultB result)
            => resilienceStrategy.Execute(static state => state.api.CallB(state.result), (api, result));

        public void CallC(ResultC result)
            => resilienceStrategy.Execute(static state => state.api.CallC(state.result), (api, result));

        public ResultA GetResultA() => resilienceStrategy.Execute(a => a.GetResultA(), api);

        public ResultB GetResultB() => resilienceStrategy.Execute(a => a.GetResultB(), api);

        public ResultC GetResultC() => resilienceStrategy.Execute(a => a.GetResultC(), api);
    }
}
