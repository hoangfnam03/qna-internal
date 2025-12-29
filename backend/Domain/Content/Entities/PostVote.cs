// Domain/Content/Entities/PostVote.cs
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class PostVote
    {
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public short Value { get; set; } // IN (-1,1)
        public DateTime CreatedAt { get; set; }
    }
}
