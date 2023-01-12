namespace BusTable.Core.Dto
{
    public class BusDepartureTimesForTheStopRequest : RequestWithLanguage, IBusRouteStopsRequest
    {
        public int CityId { get; set; } = 0;

        /// <summary>
        /// 531
        /// </summary>
        public string RouteNumber { get; set; } = string.Empty;

        /// <summary>
        /// 1439
        /// </summary>
        public int StopID { get; set; } = 0;
    }
}
