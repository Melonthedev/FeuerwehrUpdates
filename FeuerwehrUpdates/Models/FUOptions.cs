namespace FeuerwehrUpdates.Models
{
    public class FUOptions
    {
        public int CheckForChangesIntervalInMinutes { get; set; }

        public string VAPIDPublicKey { get; set; }
        public string VAPIDPrivateKey { get; set; }

        public string Subject { get; set; }

        public string DocumentUrl { get; set; }

        public string QuerySelectorLatestOperation { get; set; }

    }
}
