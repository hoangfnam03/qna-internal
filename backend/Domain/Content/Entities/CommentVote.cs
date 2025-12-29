// Domain/Content/Entities/CommentVote.cs
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class CommentVote
    {
        public Guid CommentId { get; set; }
        public Comment? Comment { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public short Value { get; set; } // IN (-1,1)
        public DateTime CreatedAt { get; set; }
    }
}
