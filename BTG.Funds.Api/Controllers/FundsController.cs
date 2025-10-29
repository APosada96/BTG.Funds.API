using BTG.Funds.Application.Services;
using BTG.Funds.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTG.Funds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundsController : ControllerBase
    {
        private readonly FundService _service;

        public FundsController(FundService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetFunds() => Ok(await _service.GetFundsAsync());

        [HttpPost("subscribe/{fundId}")]
        public async Task<IActionResult> Subscribe(string fundId, [FromBody] NotificationPreference pref)
        {
            try
            {
                var result = await _service.SubscribeAsync(fundId, pref);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("cancel/{fundId}")]
        public async Task<IActionResult> Cancel(string fundId)
        {
            try
            {
                var result = await _service.CancelAsync(fundId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory() => Ok(await _service.GetHistoryAsync());
    }
}
