namespace Phantom.CSourceQuery.Models
{
    public class SourceServer
    {
        public string Name { get; set; }
        public short Port { get; set; }
        public string Game { get; set; }
		public short AppID { get; set; }
        public string Version { get; set; }
        public string Map { get; set; }
        public byte PlayerCount { get; set; }
        public byte MaxPlayers { get; set; }
        public byte BotCount { get; set; }
        public bool Password { get; set; }
        public bool VAC { get; set; }
        public DedicatedType Dedicated { get; set; }
        public OSType OS { get; set; }
        public ulong SteamID { get; set; }
        public short SourceTVPort { get; set; }
        public string SourceTVServerName { get; set; }
        public string Keywords { get; set; }
        public ulong GameID { get; set; }

        public enum DedicatedType
        {
            NONE,
            LISTEN,
            DEDICATED,
            SOURCETV
        };

        public enum OSType
        {
            NONE,
            WINDOWS,
            LINUX
        };
    }
}
