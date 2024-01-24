namespace GmWeb.Logic.Utility.Redis
{
    public class CacheOptions
    {
        public bool Enabled { get; set; } = false;
        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 6379;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SSL { get; set; }

    }
}
