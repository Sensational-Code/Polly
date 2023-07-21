using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsageScenarios.Scenarios;

namespace UsageScenarios;

public class ScenariosBenchmark
{
    private readonly Api _rootApi = new();
    private IApi _api1;
    private IApi _api2;

    [GlobalSetup]
    public void Setup()
    {
        Scenario scenario = ScenarioName switch
        {
            "NonGeneric" => new NonGenericScenario(),
            "Generic_Object" => new GenericScenario_Object(),
            "Generic_OneOf" => new GenericScenario_OneOf(),
        };

        _api1 = scenario.WrapApi(_rootApi, true);
        _api2 = scenario.WrapApi(_rootApi, false);
    }

    [Params("NonGeneric", "Generic_Object", "Generic_OneOf")]
    public string? ScenarioName { get; set; }

    [Benchmark]
    public void ExecuteScenario()
    {
        _api1.CallA(_rootApi.ResultA);
        _api1.CallB(_rootApi.ResultB);
        _api1.CallC(_rootApi.ResultC);
        _api1.GetResultA();
        _api1.GetResultB();
        _api1.GetResultC();

        _api2.CallA(_rootApi.ResultA);
        _api2.CallB(_rootApi.ResultB);
        _api2.CallC(_rootApi.ResultC);
        _api2.GetResultA();
        _api2.GetResultB();
        _api2.GetResultC();
    }
}
