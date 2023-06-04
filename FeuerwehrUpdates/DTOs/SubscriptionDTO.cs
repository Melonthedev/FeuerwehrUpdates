namespace FeuerwehrUpdates.DTOs
{
    public class SubscriptionDTO
    {
        public Guid Id { get; set; }

        public string Endpoint { get; set; }

        public long? ExpirationTime { get; set; }

        public KeysDTO Keys { get; set; }

        public Guid KeysId { get; set; }
    }
}
