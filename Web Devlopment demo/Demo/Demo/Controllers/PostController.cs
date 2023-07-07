using Demo.Entity;
using Demo.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v1/posts")]
    //[ApiVersion("1.0")]
    public class PostController : ControllerBase
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IValidator<Post> _postValidator;

        public PostController(IRepository<Post> postRepository, IValidator<Post> postValidator)
        {
            _postRepository = postRepository;
            _postValidator = postValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(Post post)
        {
            var validationResult = await _postValidator.ValidateAsync(post);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _postRepository.AddAsync(post);
            return Ok(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Post updatedPost)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;

            var validationResult = await _postValidator.ValidateAsync(post);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _postRepository.UpdateAsync(post);

            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            await _postRepository.DeleteAsync(id);

            return Ok();
        }
    }

}
