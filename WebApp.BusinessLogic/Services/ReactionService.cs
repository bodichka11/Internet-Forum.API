using AutoMapper;
using WebApp.BusinessLogic.DTOs.Reaction;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class ReactionService : IReactionService
{
    private readonly IReactionRepository reactionRepository;
    private readonly IMapper mapper;

    public ReactionService(IReactionRepository reactionRepository, IMapper mapper)
    {
        this.reactionRepository = reactionRepository;
        this.mapper = mapper;
    }

    public async Task ReactAsync(ReactionDto reactionDto)
    {
        ValidateReactionParameters(reactionDto);

        var reaction = await this.reactionRepository.GetReactionByPostOrCommentAndUserAsync(reactionDto.PostId, reactionDto.CommentId, reactionDto.UserId);
        if (reaction != null)
        {
            if (reaction.Type == reactionDto.Type)
            {
                await this.reactionRepository.DeleteReactionAsync(reaction.Id);
            }
            else
            {
                reaction.Type = reactionDto.Type;
                await this.reactionRepository.UpdateReactionAsync(reaction);
            }
        }
        else
        {
            var newReaction = new Reaction
            {
                UserId = reactionDto.UserId,
                PostId = reactionDto.PostId,
                CommentId = reactionDto.CommentId,
                Type = reactionDto.Type,
            };
            await this.reactionRepository.AddReactionAsync(newReaction);
        }

        await this.reactionRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReactionDto>> GetReactionsByPostAsync(long postId, int pageNumber, int pageSize)
    {
        var reactions = await this.reactionRepository.GetReactionsByPostAsync(postId, pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<ReactionDto>>(reactions);
    }

    public async Task<IEnumerable<ReactionDto>> GetReactionsByCommentAsync(long commentId, int pageNumber, int pageSize)
    {
        var reactions = await this.reactionRepository.GetReactionsByCommentAsync(commentId, pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<ReactionDto>>(reactions);
    }

    private static void ValidateReactionParameters(ReactionDto reactionDto)
    {
        ArgumentNullException.ThrowIfNull(reactionDto);
    }
}
