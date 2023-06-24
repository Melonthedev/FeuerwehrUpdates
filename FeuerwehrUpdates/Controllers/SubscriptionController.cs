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
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(FWUpdatesDbContext context, ILogger<SubscriptionController> logger)
        {
            _context = context;
            _logger = logger;
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
            _logger.LogInformation($"New User has subscribed! (P256DH {subscription.Keys.p256dh})");
            return Ok();
        }
    }
}
