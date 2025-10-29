using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTG.Funds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<UserAccount> _userRepo;

        public UsersController(IRepository<UserAccount> userRepo, IRepository<Fund> fundRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var user = (await _userRepo.GetAllAsync()).FirstOrDefault();

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(user);
        }
    }
}
