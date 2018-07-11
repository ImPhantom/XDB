namespace Phantom.CSourceQuery.Models
{
    public class PlayerDetails
    {
        public string Name { get; set; }
        public int Kills { get; set; } = -9999;
        public int Deaths { get; set; } = -9999;
        public int Score { get; set; } = -9999;
        public int Ping { get; set; } = -9999;
        public int Rate { get; set; } = -9999;
        public int Index { get; set; } = -9999;
        public float Time { get; set; } = 0.0f;
    }
}
