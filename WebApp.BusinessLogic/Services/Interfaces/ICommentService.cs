using WebApp.BusinessLogic.DTOs.Comment;

namespace WebApp.BusinessLogic.Services.Interfaces;
public interface ICommentService
{
    Task<CommentDto> AddComment(CommentCreateDto commentCreateDto);

    Task<IEnumerable<CommentDto>> GetCommentsByPostId(long id, int pageNumber, int pageSize);

    Task<CommentDto> GetCommentById(long id);

    Task<CommentDto> UpdateComment(CommentDto commentDto);

    Task DeleteComment(long commentId);
}
