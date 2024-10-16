using WebApp.BusinessLogic.DTOs.Reaction;

namespace WebApp.BusinessLogic.Services.Interfaces;
public interface IReactionService
{
    Task ReactAsync(ReactionDto reactionDto);

    Task<IEnumerable<ReactionDto>> GetReactionsByPostAsync(long postId, int pageNumber, int pageSize);

    Task<IEnumerable<ReactionDto>> GetReactionsByCommentAsync(long commentId, int pageNumber, int pageSize);
}
