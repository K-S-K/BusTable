namespace BusTable.Core.Dto
{
    public class BusStopHeader
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Distance { get; set; } = 0.0;


        public override string ToString()
        {
            System.Text.StringBuilder sb = new();

            sb.Append($"[{Id}]");

            if (Distance > 0.001)
            {
                sb.Append($" <{Distance:0.00#}>");
            }

            sb.Append($" \"{Name}\"");

            return sb.ToString();
        }
    }
}
