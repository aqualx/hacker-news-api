using System.Text.Json.Serialization;
using HackerNewsApi.Converters;

namespace HackerNewsApi.Models;

public class Story
{
    public string Title { get; set; } = string.Empty;
    public string PostedBy { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime Time { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
}