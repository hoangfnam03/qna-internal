// Domain/Content/Entities/Comment.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class Comment : SoftDeletableEntity
    {
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public string Body { get; set; } = default!;
        public int Score { get; set; }

        // spec: NOT NULL
        public new Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public new Guid? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }

        public ICollection<CommentRevision> Revisions { get; set; } = new List<CommentRevision>();
        public ICollection<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();
        public ICollection<CommentVote> Votes { get; set; } = new List<CommentVote>();
    }
}
