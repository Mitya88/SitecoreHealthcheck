namespace Healthcheck.Service.Models
{
    public class CacheStatisticsResponse
    {
        public long FullCacheSize { get; set; }

        public long UsedCacheSize { get; set; }

        public string FullCacheSizeText { get; set; }

        public string UsedCacheText { get; set; }
    }
}