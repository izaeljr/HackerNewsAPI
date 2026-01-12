using HackerNewsAPI.Configuration;
using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace HackerNewsAPI.Services
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly HackerNewsOptions _options;
        private readonly SemaphoreSlim _semaphore;
        
        public HackerNewsClient(HttpClient httpClient, IOptions<HackerNewsOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _semaphore = new SemaphoreSlim(_options.MaxConcurrency);
        }

        public async Task<List<StoryResponse>> GetBestStoriesAsync(int n)
        {
            var storyIds = await _httpClient.
                GetFromJsonAsync<List<int>>($"{_options.BaseUrl}/beststories.json");

            var tasks = storyIds!.Take(n * 3).Select(GetStoryAsync);
        }

        private async Task<StoryResponse?> GetStoryAsync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var item = await _httpClient
                    .GetFromJsonAsync<HackerNewsItem>($"{_options.BaseUrl}item{id}.json");

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
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
