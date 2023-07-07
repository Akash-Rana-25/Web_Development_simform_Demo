using Demo.Entity;
using Demo.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v1/comments")]
    //[ApiVersion("1.0")]
    public class CommentController : ControllerBase
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IValidator<Comment> _commentValidator;

        public CommentController(IRepository<Comment> commentRepository, IValidator<Comment> commentValidator)
        {
            _commentRepository = commentRepository;
            _commentValidator = commentValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            var comments = await _commentRepository.GetAllAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            var validationResult = await _commentValidator.ValidateAsync(comment);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _commentRepository.AddAsync(comment);
            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, Comment updatedComment)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            //comment.Content = updatedComment.Content;

            var validationResult = await _commentValidator.ValidateAsync(comment);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _commentRepository.UpdateAsync(comment);

            return Ok(comment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteAsync(id);

            return Ok();
        }
    }


}
