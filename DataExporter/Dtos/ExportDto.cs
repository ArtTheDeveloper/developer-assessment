using System.ComponentModel.DataAnnotations;

namespace DataExporter.Dtos
{
    public class ExportDto
    {
        public string? PolicyNumber { get; set; }
        public decimal Premium { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // A list of the notes' text.
        public IList<string> Notes { get; set; } = new List<string>();
    }
}
