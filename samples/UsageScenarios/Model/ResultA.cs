public record class ResultA(string Value)
{
    public bool IsError => Value == "Error";
}
