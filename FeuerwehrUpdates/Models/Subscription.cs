namespace FeuerwehrUpdates.Models
{
    public class Subscription
    {
        public string Endpoint { get; set; }

        public long? ExpirationTime { get; set; }

        public Keys Keys { get; set; }
    }
}
