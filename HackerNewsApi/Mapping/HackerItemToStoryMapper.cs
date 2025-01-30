using HackerNewsApi.Models;

namespace HackerNewsApi.Mapping;

public static class HackerItemToStoryMapper
{
    public static Story ToStory(this HackerItem hackerItem)
    {
        return new Story
        {
            Title = hackerItem.Title,
            Uri = hackerItem.Url,
            PostedBy = hackerItem.UserName,
            Time = hackerItem.Time,
            Score = hackerItem.Score,
            CommentCount = hackerItem.CommentCount
        };
    }
}