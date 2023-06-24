using FeuerwehrUpdates.DTOs;
using FeuerwehrUpdates.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebPush;

namespace FeuerwehrUpdates.Services
{
    public class PushService
    {

        private readonly FWUpdatesDbContext _context;
        private readonly FUOptions _options;
        private readonly ILogger<PushService> _logger;

        public PushService(IDbContextFactory<FWUpdatesDbContext> contextFactory, IOptions<FUOptions> options, ILogger<PushService> logger)
        {
            _context = contextFactory.CreateDbContext();
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendPushNotificationToAll(Payload payload)
        {
            await _context.Subscriptions.ForEachAsync(async subscription => await SendPushNotification(subscription, payload));
        }

        public async Task SendPushNotification(SubscriptionDTO subscription, Payload payload)
        {
            var subject = _options.Subject;
            var publicKey = _options.VAPIDPublicKey;
            var privateKey = _options.VAPIDPrivateKey;

            var pushsubscription = new PushSubscription(subscription.Endpoint, subscription.Keys.p256dh, subscription.Keys.auth);
            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            var webPushClient = new WebPushClient();

            try
            {
                await webPushClient.SendNotificationAsync(pushsubscription, JsonConvert.SerializeObject(payload), vapidDetails);
                _logger.LogInformation($"Sent Push Notification! (P256DH {pushsubscription.P256DH})");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
