using Polly;

public interface IApi
{
    ResultA GetResultA();

    ResultB GetResultB();

    ResultC GetResultC();

    public void CallA(ResultA result);

    public void CallB(ResultB result);

    public void CallC(ResultC result);
}
