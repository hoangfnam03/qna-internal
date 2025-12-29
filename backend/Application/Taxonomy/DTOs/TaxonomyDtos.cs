namespace Application.Taxonomy.DTOs
{
    public record CategoryDto(
        Guid Id,
        string Name,
        string Slug,
        Guid? ParentId,
        int SortOrder,
        bool IsHidden
    );

    public record TagDto(
        Guid Id,
        string Name,
        string Slug
    );

    public record CreateCategoryRequest(
        string Name,
        string? Slug,
        Guid? ParentId,
        int SortOrder = 0,
        bool IsHidden = false
    );

    public record UpdateCategoryRequest(
        string Name,
        string? Slug,
        Guid? ParentId,
        int SortOrder = 0,
        bool IsHidden = false
    );

    public record CreateTagRequest(string Name, string? Slug);
    public record UpdateTagRequest(string Name, string? Slug);
}
