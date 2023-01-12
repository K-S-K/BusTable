using System.Text;

namespace BusTable.Core.Dto
{
    public class BusRoutesRequest : RequestWithLanguage
    {
        #region -> Data
        private int _maxPageSize = int.MaxValue;
        private int _minPageSize = 5;
        private int _pageSize = int.MaxValue;
        private int _minPageNumber = 1;
        private int _pageNumber = 1;
        #endregion

        public int CityId { get; set; } = 0;

        public string Search { get; set; } = string.Empty;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = Math.Max(_minPageNumber, value);
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < _minPageSize)
                {
                    _pageSize = _minPageSize;
                }
                else if (value > _maxPageSize)
                {
                    _pageSize = _maxPageSize;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                sb.Append($"{nameof(Search)}: \"{Search}\"");
            }

            if (PageSize != int.MaxValue)
            {
                sb.Append($"${nameof(PageSize)}: {PageSize}");
            }

            if (PageNumber != 1 || PageSize != int.MaxValue)
            {
                sb.Append($"${nameof(PageNumber)}: {PageNumber}");
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
