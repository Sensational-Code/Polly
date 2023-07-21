public record class ResultB(string Value)
{
    public bool IsError => Value == "Error";
}
