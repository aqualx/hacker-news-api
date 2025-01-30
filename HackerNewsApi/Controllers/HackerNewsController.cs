using HackerNewsApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsApiClient _hackerApiClient;
    private readonly ILogger<HackerNewsController> _logger;

    public HackerNewsController(IHackerNewsApiClient hackerApiClient, ILogger<HackerNewsController> logger)
    {
        _hackerApiClient = hackerApiClient;
        _logger = logger;
    }

    [HttpGet("/best-stories")]
    public async Task<IActionResult> GetBestStories(CancellationToken token, [FromQuery] int count = 10, [FromQuery] string order = "desc")
    {
        _logger.LogInformation("Received request for best stories with count={Count}, order={Order}", count, order);
        if (count <= 0 || count > 500)
        {
            _logger.LogWarning("Invalid count={Count} requested. Must be between 1-500.", count);
            return BadRequest("Invalid number of stories requested. Valid number is: 1-500");
        }

        try
        {
            var stories = await _hackerApiClient.GetBestStoriesAsync(count, token);
            if (stories == null || !stories.Any())
            {
                var msg = "No stories found.";
                _logger.LogWarning(msg);
                return NotFound(msg);
            }

            stories = order?.ToLower() == "asc"
                ? stories.OrderBy(s => s.Score).ToList()
                : stories.OrderByDescending(s => s.Score).ToList();
            _logger.LogInformation("Returning {Count} sorted stories.", stories.Count);
            return Ok(stories);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Request was canceled by the client.");
            return BadRequest("Request was canceled.");
        }
    }
}