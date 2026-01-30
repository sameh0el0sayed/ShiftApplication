namespace ShiftApplication.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ManpowerLog
    {
        public int Id { get; set; }

        [Required]
        public int ShiftId { get; set; }
        public Shift Shift { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string Description { get; set; }
    }

}
