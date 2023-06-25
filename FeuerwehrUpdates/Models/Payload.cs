namespace FeuerwehrUpdates.Models
{
    public class Payload
    {
        public Guid Id { get; set; }
        public string OperationId { get; set; }

        public string Tag { get; set; }

        public string Title { get; set; }

        public string? Content { get; set; }

        public string? PressLink { get; set; }
    }
}
