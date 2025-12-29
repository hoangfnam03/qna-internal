namespace Application.Posts.DTOs
{
    public record PostTagDto(Guid Id, string Name, string Slug);

    public record PostListItemDto(
        Guid Id,
        string Title,
        string Excerpt,
        int Score,
        int CommentCount,
        DateTime CreatedAt,
        Guid AuthorId,
        string AuthorDisplayName,
        Guid? CategoryId,
        string? CategoryName,
        List<PostTagDto> Tags
    );

    public record PostDetailDto(
        Guid Id,
        string Title,
        string Body,
        int Score,
        int CommentCount,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        Guid AuthorId,
        string AuthorDisplayName,
        Guid? CategoryId,
        string? CategoryName,
        List<PostTagDto> Tags
    );

    public record CreatePostRequest(
        string Title,
        string Body,
        Guid? CategoryId,
        List<Guid>? TagIds
    );

    public record UpdatePostRequest(
        string Title,
        string Body,
        Guid? CategoryId,
        List<Guid>? TagIds,
        string? Summary
    );

    public record PostRevisionDto(
        Guid Id,
        Guid PostId,
        string EditorDisplayName,
        DateTime CreatedAt,
        string? Summary,
        string? BeforeTitle,
        string? AfterTitle,
        string? BeforeBody,
        string? AfterBody
    );
}
