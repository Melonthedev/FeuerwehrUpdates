using FeuerwehrUpdates.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;

namespace FeuerwehrUpdates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {

        [HttpPost("save")]
        public IActionResult SaveSubscription([FromBody] Subscription subscription) {
            var subject = @"https://FeuerwehrUpdates.melonthedev.wtf";
            var publicKey = @"BH-9RrroeoEqN37m3SHxOVU97dSOEud8mrkyFAp-O8clW3zNdHjvfwfZ6vwkiR61gob7UqQALHOEoG57qTaK6B4";
            var privateKey = @"XjxriNWDQZh6PrGzPfDXkyrJ_TrFUBOSOJiNuBbJlHI";

            var payload = JsonConvert.SerializeObject(new Payload() {
                Title = "Einsatz: COCK",
                Content = "blablablabla egretrhhrtgerhet erhg ehrhrehrehre",
                Tag = "neuer-einsatz",
                PressLink = "https://osthessen-news.de/"
            });

            var pushsubscription = new PushSubscription(subscription.endpoint, subscription.keys.p256dh, subscription.keys.auth);
            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            var webPushClient = new WebPushClient();

            webPushClient.SendNotification(pushsubscription, payload, vapidDetails);
            return Ok();
        }
    }
}
