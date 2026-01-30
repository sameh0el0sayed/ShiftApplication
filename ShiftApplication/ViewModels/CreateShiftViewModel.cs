using System.ComponentModel.DataAnnotations;

namespace ShiftApplication.ViewModels
{
    public class CreateShiftViewModel
    {
        [Required]
        [Display(Name = "Shift Start Time")]
        public DateTime StartTime { get; set; } = DateTime.Now;
    }
}
