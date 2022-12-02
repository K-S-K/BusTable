namespace BusTable.Core.Dto
{
    public interface IBusRouteStopsRequest
    {
        int CityId { get; set; }
        string Language { get; set; }

        /// <summary>
        /// 531
        /// </summary>
        string RouteNumber { get; set; }
    }
}