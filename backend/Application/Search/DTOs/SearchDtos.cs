using Application.Common.Models;

namespace Application.Search.DTOs
{
    public record PostSearchItemDto(
        Guid Id,
        string Title,
        string Excerpt,
        int Score,
        int CommentCount,
        DateTime CreatedAt,
        Guid AuthorId,
        string AuthorDisplayName
    );

    public record UserSearchItemDto(
        Guid Id,
        string UserName,
        string DisplayName,
        string? Email
    );

    public record TagSearchItemDto(
        Guid Id,
        string Name,
        string Slug
    );

    public record SearchResponseDto(
        Paged<PostSearchItemDto> Posts,
        IReadOnlyList<UserSearchItemDto> Users,
        IReadOnlyList<TagSearchItemDto> Tags
    );
}
