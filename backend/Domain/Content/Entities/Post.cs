// Domain/Content/Entities/Post.cs
using Domain.Common.Entities;
using Domain.Content.Taxonomy;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class Post : SoftDeletableEntity
    {
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;

        public int Score { get; set; }
        public int CommentCount { get; set; }

        // spec: NOT NULL
        public new Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public new Guid? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }

        public ICollection<PostRevision> Revisions { get; set; } = new List<PostRevision>();
        public ICollection<PostAttachment> Attachments { get; set; } = new List<PostAttachment>();
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public ICollection<PostVote> Votes { get; set; } = new List<PostVote>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
