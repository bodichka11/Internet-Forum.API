using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApp.BusinessLogic.DTOs.Comment;
using WebApp.BusinessLogic.Helpers;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository commentRepository;
    private readonly IMapper mapper;
    private readonly ILogger<CommentService> logger;

    public CommentService(ICommentRepository commentRepository, IMapper mapper, ILogger<CommentService> logger)
    {
        this.commentRepository = commentRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<CommentDto> AddComment(CommentCreateDto commentCreateDto)
    {
        var comment = this.mapper.Map<Comment>(commentCreateDto);
        comment.CreatedAt = DateTime.UtcNow;

        await this.commentRepository.AddCommentAsync(comment);
        await this.commentRepository.SaveChangesAsync();

        var createdComment = this.mapper.Map<CommentDto>(comment);
        CommentServiceLogging.CommentAdded(this.logger, createdComment.Id);

        return createdComment;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostId(long id, int pageNumber, int pageSize)
    {
        var comment = await this.commentRepository.GetCommentsByPostIdAsync(id, pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<CommentDto>>(comment);
    }

    public async Task<CommentDto> GetCommentById(long id)
    {
        var comment = await this.commentRepository.GetCommentById(id);
        return this.mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> UpdateComment(CommentDto commentDto)
    {
        CheckCommentParameters(commentDto);

        var comment = await this.commentRepository.GetCommentByIdAsync(commentDto.Id);
        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found");
        }

        comment.Content = commentDto.Content;

        _ = this.commentRepository.UpdateCommentAsync(comment);

        var updatedComment = this.mapper.Map<CommentDto>(comment);
        CommentServiceLogging.CommentUpdated(this.logger, updatedComment.Id);

        return updatedComment;
    }

    public async Task DeleteComment(long commentId)
    {
        var comment = await this.commentRepository.GetCommentByIdAsync(commentId);
        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found");
        }

        await this.commentRepository.DeleteCommentAsync(commentId);
        await this.commentRepository.SaveChangesAsync();

        CommentServiceLogging.CommentDeleted(this.logger, commentId);
    }

    private static void CheckCommentParameters(CommentDto commentDto)
    {
        ArgumentNullException.ThrowIfNull(commentDto);
    }
}
