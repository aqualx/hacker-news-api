using HackerNewsApi.Models;

namespace HackerNewsApi.Interfaces;

public interface IHackerNewsApiClient
{
    Task<List<Story>> GetBestStoriesAsync(int count, CancellationToken token);
}