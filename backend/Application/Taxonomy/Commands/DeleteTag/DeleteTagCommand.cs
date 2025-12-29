using MediatR;

namespace Application.Taxonomy.Commands.DeleteTag
{
    public record DeleteTagCommand(Guid Id) : IRequest<bool>;
}
