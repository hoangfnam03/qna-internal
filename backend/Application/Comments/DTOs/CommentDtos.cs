namespace Application.Comments.DTOs
{
    public record CommentItemDto(
        Guid Id,
        Guid PostId,
        Guid? ParentCommentId,
        string Body,
        int Score,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        Guid AuthorId,
        string AuthorDisplayName
    );

    public record CreateCommentRequest(Guid? ParentCommentId, string Body);

    public record UpdateCommentRequest(string Body, string? Summary);

    public record CommentRevisionDto(
        Guid Id,
        Guid CommentId,
        string EditorDisplayName,
        DateTime CreatedAt,
        string? Summary,
        string? BeforeBody,
        string? AfterBody
    );

    public record VoteRequest(short Value); // -1 or +1
}
