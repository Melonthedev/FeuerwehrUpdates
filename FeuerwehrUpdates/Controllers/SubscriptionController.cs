using FeuerwehrUpdates.DTOs;
using FeuerwehrUpdates.Models;
using FeuerwehrUpdates.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeuerwehrUpdates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {

        private readonly FWUpdatesDbContext _context;

        public SubscriptionController(FWUpdatesDbContext context)
        {
            _context = context;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveSubscription([FromBody] Subscription subscription) {
            SubscriptionDTO subscriptionDTO = new()
            {
                Endpoint = subscription.Endpoint,
                ExpirationTime = subscription.ExpirationTime,
                Keys = new KeysDTO()
                {
                    auth = subscription.Keys.auth,
                    p256dh = subscription.Keys.p256dh
                }
            };
            _context.Subscriptions.Add(subscriptionDTO);
            await _context.SaveChangesAsync();
            await Console.Out.WriteLineAsync("New User has subscribed!");
            return Ok();
        }
    }
}
