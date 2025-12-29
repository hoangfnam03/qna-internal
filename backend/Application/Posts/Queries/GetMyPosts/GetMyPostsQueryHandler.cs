using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Posts.DTOs;
using Application.Posts.Queries.GetPostFeed;
using MediatR;

namespace Application.Posts.Queries.GetMyPosts
{
    public class GetMyPostsQueryHandler : IRequestHandler<GetMyPostsQuery, Paged<PostFeedItemDto>>
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _current;

        public GetMyPostsQueryHandler(IMediator mediator, ICurrentUserService current)
        {
            _mediator = mediator;
            _current = current;
        }

        public async Task<Paged<PostFeedItemDto>> Handle(GetMyPostsQuery request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            return await _mediator.Send(new GetPostFeedQuery("recent", null, _current.UserId.Value, request.Page, request.PageSize), ct);
        }
    }
}
