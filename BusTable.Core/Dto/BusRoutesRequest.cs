using System.Text;

namespace BusTable.Core.Dto
{
    public class BusRoutesRequest : RequestWithPagination
    {
        public int CityId { get; set; } = 0;

        public string Search { get; set; } = string.Empty;

        public override string ToString()
        {
            StringBuilder sb = new();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                sb.Append($"{nameof(Search)}: \"{Search}\"");
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
