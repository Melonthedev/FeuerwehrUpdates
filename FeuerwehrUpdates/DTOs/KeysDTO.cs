namespace FeuerwehrUpdates.DTOs
{
    public class KeysDTO
    {
        public Guid Id { get; set; }

        public string p256dh { get; set; }

        public string auth { get; set; }

        public SubscriptionDTO Subscription { get; set; }
    }
}
