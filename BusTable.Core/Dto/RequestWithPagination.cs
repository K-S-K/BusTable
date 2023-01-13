using System.Text;

namespace BusTable.Core.Dto
{
    public abstract class RequestWithPagination : RequestWithLanguage
    {
        #region -> Data
        protected int _maxPageSize = int.MaxValue;
        protected int _minPageSize = 3;
        protected int _pageSize = int.MaxValue;
        protected int _minPageNumber = 1;
        protected int _pageNumber = 1;
        #endregion

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
