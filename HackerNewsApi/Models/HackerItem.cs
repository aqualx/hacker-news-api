using System.Text.Json.Serialization;
using HackerNewsApi.Converters;

namespace HackerNewsApi.Models;

public class HackerItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; } = false;

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HackerItemType Type { get; set; }

    [JsonPropertyName("by")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime Time { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("dead")]
    public bool Dead { get; set; } = false;

    [JsonPropertyName("parent")]
    public int? ParentId { get; set; }

    [JsonPropertyName("poll")]
    public int? PollId { get; set; }

    [JsonPropertyName("kids")]
    public List<int>? ChildrenIds { get; set; } = default;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public int Score { get; set; } = 0;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("parts")]
    public List<int>? RelatedPollOuts { get; set; } = default;

    [JsonPropertyName("descendants")]
    public int CommentCount { get; set; }

    public enum HackerItemType
    {
        job,
        story,
        comment,
        poll,
        pollopt,
    }
}