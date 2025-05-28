namespace DataExporter.Dtos
{
    public class ExportDto
    {
        public string? PolicyNumber { get; set; }
        public decimal Premium { get; set; }
        public DateTime StartDate { get; set; }

        // A list of the notes' text.
        public IList<string> Notes { get; set; } = new List<string>();
    }
}
