namespace Application.Posts.DTOs
{
    public record PostFeedItemDto(
        Guid Id,
        string Title,
        string Excerpt,
        int Score,
        int CommentCount,
        DateTime CreatedAt,
        Guid AuthorId,
        string AuthorDisplayName
    );
}
