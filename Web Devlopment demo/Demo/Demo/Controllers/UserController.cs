using Demo.Entity;
using Demo.Repository;
using Demo.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/v1/users")]
    //[ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UserController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var validator = new UserValidator();
            var validationResult = await validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _userRepository.AddAsync(user);
            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;

            await _userRepository.UpdateAsync(user);

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(id);

            return Ok();
        }
    }

}
