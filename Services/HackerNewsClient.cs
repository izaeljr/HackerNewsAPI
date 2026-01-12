using HackerNewsAPI.Configuration;
using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNewsAPI.Services
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly HackerNewsOptions _options;
        private readonly IMemoryCache _cache;

        public HackerNewsClient(
            HttpClient httpClient,
            IOptions<HackerNewsOptions> options,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _cache = cache;
        }

        public async Task<List<StoryResponse>> GetBestStoriesAsync(int n)
        {
            var cacheKey = $"best-stories-{n}";

            if (_cache.TryGetValue(cacheKey, out List<StoryResponse>? cached))
                return cached!;

            var storyIds = await _httpClient
                .GetFromJsonAsync<List<int>>($"{_options.BaseUrl}/beststories.json");

            var queue = new PriorityQueue<StoryResponse, int>();
            var sync = new object();

            await Parallel.ForEachAsync(
                storyIds!,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxConcurrency
                },
                async (id, _) =>
                {
                    var story = await GetStoryAsync(id);
                    if (story == null) return;

                    lock (sync)
                    {
                        if (queue.Count < n)
                        {
                            queue.Enqueue(story, story.Score);
                        }
                        else if (story.Score > queue.Peek().Score)
                        {
                            queue.Dequeue();
                            queue.Enqueue(story, story.Score);
                        }
                    }
                });

            var result = queue
                .UnorderedItems
                .Select(x => x.Element)
                .OrderByDescending(s => s.Score)
                .ToList();

            _cache.Set(
                cacheKey,
                result,
                TimeSpan.FromMinutes(_options.CacheMinutes)
            );

            return result;
        }

        private async Task<StoryResponse?> GetStoryAsync(int id)
        {
            var item = await _httpClient
                .GetFromJsonAsync<HackerNewsItem>(
                    $"{_options.BaseUrl}/item/{id}.json");

            if (item?.Title == null || item.Url == null)
                return null;

            return new StoryResponse(
                item.Title,
                item.Url,
                item.By!,
                DateTimeOffset.FromUnixTimeSeconds(item.Time),
                item.Score,
                item.Descendants
            );
        }
    }
}
