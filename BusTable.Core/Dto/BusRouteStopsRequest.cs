namespace BusTable.Core.Dto
{
    public class BusRouteStopsRequest : IBusRouteStopsRequest
    {
        public string Language { get; set; } = "ANY";

        public int CityId { get; set; } = 0;

        /// <summary>
        /// 531
        /// </summary>
        public string RouteNumber { get; set; } = string.Empty;
    }
}
