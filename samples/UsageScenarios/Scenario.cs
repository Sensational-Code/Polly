namespace UsageScenarios;

internal abstract class Scenario
{
    public abstract IApi WrapApi(IApi originalApi, bool onlyExceptions);
}
