using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using System.Net.Http.Json;

namespace HackerNewsAPI.Services
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private const string _baseUrl = "https://hacker-news.firebaseio.com/v0/";
        private readonly HttpClient _httpClient;
        
        public HackerNewsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StoryResponse>> GetBestStoriesAsync(int n)
        {
            var storyIds = await _httpClient.
                GetFromJsonAsync<List<int>>($"{_baseUrl}/beststories.json");

        }
    }
}
