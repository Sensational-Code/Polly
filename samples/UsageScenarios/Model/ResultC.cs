public record class ResultC(string Value)
{
    public bool IsError => Value == "Error";
}
