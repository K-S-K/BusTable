namespace BusTable.Core.Dto
{
    public class BusRoutesRequest
    {
        #region -> Data
        private int _maxPageSize = int.MaxValue;
        private int _minPageSize = 5;
        private int _pageSize = int.MaxValue;
        private int _minPageNumber = 1;
        private int _pageNumber = 1;
        #endregion

        public string Language { get; set; } = "ANY";

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
    }
}
