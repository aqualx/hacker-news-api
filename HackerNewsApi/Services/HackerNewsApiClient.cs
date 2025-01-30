using HackerNewsApi.Interfaces;
using HackerNewsApi.Mapping;
using HackerNewsApi.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace HackerNewsApi.Services;

public class HackerNewsApiClient : IHackerNewsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly HybridCache _hybridCache;
    private readonly ILogger<HackerNewsApiClient> _logger;
    private readonly HybridCacheEntryOptions _idsEntryOption = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };
    private readonly HybridCacheEntryOptions _storyEntryOption = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(60),
        LocalCacheExpiration = TimeSpan.FromMinutes(60)
    };

    private static readonly string BestStoriesUrl = "beststories.json";
    private static readonly string ItemUrl = "item/{0}.json";

    public HackerNewsApiClient(HttpClient httpClient, HybridCache hybridCache, ILogger<HackerNewsApiClient> logger)
    {
        _httpClient = httpClient;
        _hybridCache = hybridCache;
        _logger = logger;
    }

    public async Task<List<Story>> GetBestStoriesAsync(int count, CancellationToken token)
    {
        _logger.LogInformation("Fetching top {Count} best stories...", count);

        try
        {
            var storyIds = await GetBestStoryIdsAsync(token);
            if (storyIds is null)
            {
                _logger.LogWarning("Failed to retrieve best story IDs.");
                return new List<Story>();
            }

            var validStories = new List<HackerItem>();

            foreach (var id in storyIds)
            {
                var story = await GetHackerItemAsync(id, token);

                if (story is not null && !story.Deleted && !story.Dead && story.Type == HackerItem.HackerItemType.story)
                {
                    validStories.Add(story);
                }

                if (validStories.Count == count)
                {
                    break;
                }
            }

            _logger.LogInformation("Returning {Count} stories.", validStories.Count);
            return validStories.Select(x => x.ToStory()).ToList();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request to fetch best stories was canceled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving best stories.");
            throw;
        }
    }

    private async Task<int[]?> GetBestStoryIdsAsync(CancellationToken token)
    {
        _logger.LogInformation("Fetching best story IDs...");

        var idsCacheKey = "beststories";
        var storyIds = await _hybridCache.GetOrCreateAsync<int[]?>(
            idsCacheKey,
            async _ =>
            {
                _logger.LogInformation("Best story IDs not found in cache. Fetching from API...");
                return await _httpClient.GetFromJsonAsync<int[]>(BestStoriesUrl);
            },
            _idsEntryOption,
            cancellationToken: token
        );

        if (storyIds != null)
        {
            _logger.LogInformation("Fetched {Count} story IDs.", storyIds.Length);
        }
        else
        {
            _logger.LogWarning("Fetched story IDs are null.");
        }

        return storyIds;
    }

    private async Task<HackerItem?> GetHackerItemAsync(int id, CancellationToken token)
    {
        _logger.LogInformation("Fetching HackerItem:{Id}...", id);
        var storyCacheKey = $"hackerItem:{id}";
        var story = await _hybridCache.GetOrCreateAsync<HackerItem?>(
            storyCacheKey,
            async _ =>
            {
                _logger.LogInformation("HackerItem:{Id} not found in cache. Fetching from API...", id);
                return await _httpClient.GetFromJsonAsync<HackerItem>(string.Format(ItemUrl, id));
            },
            _storyEntryOption,
            cancellationToken: token
        );

        if (story != null)
        {
            _logger.LogInformation("HackerItem:{Id} fetched successfully.", id);
        }
        else
        {
            _logger.LogWarning("HackerItem:{Id} returned null.", id);
        }

        return story;
    }
}
