namespace FeuerwehrUpdates.Models
{
    public class Einsatz
    {
        public string Id { get; set; }
        public string? Date { get; set; }
        public string? StartedTime { get; set; }
        public string? EndTime { get; set; }
        public string? EinsatzInfo { get; set; }
        public string? Location { get; set; }
        public string? Vehicles { get; set; }
        public string? EinsatzSchleifen { get; set; }
        public string? PressLink { get; set; }
    }
}
