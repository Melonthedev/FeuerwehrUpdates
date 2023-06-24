namespace FeuerwehrUpdates.Models
{
    public class FUOptions
    {
        public int CheckForChangesIntervalInMinutes { get; set; }

        public string VAPIDPublicKey { get; set; }
        public string VAPIDPrivateKey { get; set; }

        public string Subject { get; set; }

        public List<FUDocument> Documents { get; set; }

    }

    public class FUDocument
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
        public string QuerySelectorLatestOperation { get; set; }
        public string IdSelector { get; set; }
        public string DateSelector { get; set; }
        public string StartTimeSelector { get; set; }
        public string EndTimeSelector { get; set; }
        public string InfoSelector { get; set; }
        public string LocationSelector { get; set; }
        public string VehiclesSelector { get; set; }
        public string SchleifenSelector { get; set; }
        public string PressLinkSelector { get; set; }
    }
}
