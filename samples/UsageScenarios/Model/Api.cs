public class Api : IApi
{
    public Api()
    {
        ResultA = new ResultA("A");
        ResultB = new ResultB("B");
        ResultC = new ResultC("C");
    }

    public ResultA ResultA { get; }

    public ResultB ResultB { get; }

    public ResultC ResultC { get; }

    public void CallA(ResultA result) { }

    public void CallB(ResultB result) { }

    public void CallC(ResultC result) { }

    public ResultA GetResultA() => ResultA;

    public ResultB GetResultB() => ResultB;

    public ResultC GetResultC() => ResultC;
}
