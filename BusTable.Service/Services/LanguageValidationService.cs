namespace BusTable.Service.Services
{
    public class LanguageValidationService
    {
        public void Validate(string language)
        {
            language = language.Trim();

            if (string.IsNullOrEmpty(language))
            {
                throw new Exception($"The language must be provided in the request");
            }

            // TODO: It must be from the Data Layer
            if (!new BusTable.Core.Dto.LanguageData().Languages.Contains(language.ToUpper()))
            {
                throw new Exception($"There is no data registered for the language \"{language}\"");
            }
        }
    }
}
