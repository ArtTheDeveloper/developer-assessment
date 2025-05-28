using System.ComponentModel.DataAnnotations;

namespace DataExporter.Dtos
{
    public class CreatePolicyDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string PolicyNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Premium { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
    }
}
