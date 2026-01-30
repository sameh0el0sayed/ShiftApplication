using System.ComponentModel.DataAnnotations;

namespace ShiftApplication.ViewModels
{
    public class ShiftClaimViewModel
    {
        public int ShiftId { get; set; }

        [Display(Name = "Shift Start Time")]
        public DateTime StartTime { get; set; }

        public bool IsClaimed { get; set; }
    }
}
