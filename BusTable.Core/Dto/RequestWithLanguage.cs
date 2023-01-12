namespace BusTable.Core.Dto
{
    public abstract class RequestWithLanguage
    {
        public string Language { get; set; } = "ANY";

        public override string ToString() => $"Lang: {Language}";
    }
}
