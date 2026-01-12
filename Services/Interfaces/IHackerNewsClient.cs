using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services.Interfaces
{
    public interface IHackerNewsClient
    {
        Task<List<StoryResponse>> GetBestStoriesAsync(int n);
    }
}
