namespace FeuerwehrUpdates.Models
{
    public class Subscription
    {
        public string endpoint { get; set; }

        public long? expirationTime { get; set; }

        public Keys keys { get; set; }
    }
}
