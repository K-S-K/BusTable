namespace BusTable.Core.Dto
{
    public class LanguageData
    {
        // TODO: It must be from the Data Layer
        public List<string> Languages => new[] {"ANY", "EN", "GE", "RU" }.ToList();
    }
}
