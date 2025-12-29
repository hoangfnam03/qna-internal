// Domain/Content/Taxonomy/Category.cs
using Domain.Common.Entities;

namespace Domain.Content.Taxonomy
{
    public class Category : SoftDeletableEntity
    {
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;

        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        public int SortOrder { get; set; }
        public bool IsHidden { get; set; }
    }
}
