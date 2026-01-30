using System.ComponentModel.DataAnnotations;

namespace ShiftApplication.ViewModels
{
    public class ShiftLogViewModel
    {
        [Required]
        public int ShiftId { get; set; }

        [Required]
        [Display(Name = "Log Type")]
        public LogType LogType { get; set; }

        [Required]
        [Display(Name = "Date & Time")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public enum LogType
    {
        Accident = 1,
        Incident = 2,
        Manpower = 3
    }
}
