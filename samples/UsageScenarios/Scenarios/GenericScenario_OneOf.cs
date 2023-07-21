using OneOf;
using Polly;
using UsageScenarios;

namespace UsageScenarios.Scenarios;

internal class GenericScenario_OneOf : Scenario
{
    public override IApi WrapApi(IApi originalApi, bool onlyExceptions)
    {
        if (onlyExceptions)
        {
            var strategy = new ResilienceStrategyBuilder<OneOf<ResultA, ResultB, ResultC>>()
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
            var strategy = new ResilienceStrategyBuilder<OneOf<ResultA, ResultB, ResultC>>()
                .AddRetry(new()
                {
                    ShouldHandle = args => args switch
                    {
                        { Exception: HttpRequestException } => PredicateResult.True,
                        { Result: { Index: 0, AsT0: { IsError: true } } } => PredicateResult.True,
                        { Result: { Index: 1, AsT1: { IsError: true } } } => PredicateResult.True,
                        { Result: { Index: 2, AsT2: { IsError: true } } } => PredicateResult.True,
                        _ => PredicateResult.False,
                    }
                })
                .AddAdvancedCircuitBreaker(new()
                {
                    ShouldHandle = args => args switch
                    {
                        { Exception: HttpRequestException } => PredicateResult.True,
                        { Result: { Index: 0, AsT0: { IsError: true } } } => PredicateResult.True,
                        { Result: { Index: 1, AsT1: { IsError: true } } } => PredicateResult.True,
                        { Result: { Index: 2, AsT2: { IsError: true } } } => PredicateResult.True,
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
        private readonly ResilienceStrategy<OneOf<ResultA, ResultB, ResultC>> resilienceStrategy;

        public ApiImpl(IApi api, ResilienceStrategy<OneOf<ResultA, ResultB, ResultC>> resilienceStrategy)
        {
            this.api = api;
            this.resilienceStrategy = resilienceStrategy;
        }

        public void CallA(ResultA result) => resilienceStrategy.Execute(static state =>
        {
            state.api.CallA(state.result);
            return state.result;
        },
        (api, result));

        public void CallB(ResultB result) => resilienceStrategy.Execute(static state =>
        {
            state.api.CallB(state.result);
            return state.result;
        },
        (api, result));

        public void CallC(ResultC result) => resilienceStrategy.Execute(static state =>
        {
            state.api.CallC(state.result);
            return state.result;
        },
        (api, result));

        public ResultA GetResultA() => resilienceStrategy.Execute(a => a.GetResultA(), api).AsT0;

        public ResultB GetResultB() => resilienceStrategy.Execute(a => a.GetResultB(), api).AsT1;

        public ResultC GetResultC() => resilienceStrategy.Execute(a => a.GetResultC(), api).AsT2;
    }
}
