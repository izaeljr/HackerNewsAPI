namespace HackerNewsAPI.Configuration
{
    public class HackerNewsOptions
    {
        public string? BaseUrl { get; set; } = string.Empty;
        public int MaxConcurrency { get; set; }
    }
}
