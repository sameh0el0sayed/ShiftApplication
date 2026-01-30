namespace ShiftApplication.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class Shift
    {
        public int Id { get; set; }

        [Required]
        public string SupervisorId { get; set; }
        public IdentityUser Supervisor { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsClaimed { get; set; } = false;

        public bool IsClosed { get; set; } = false;

        // Navigation properties
        public List<AccidentLog> Accidents { get; set; } = new();
        public List<IncidentLog> Incidents { get; set; } = new();
        public List<ManpowerLog> ManpowerDetails { get; set; } = new();
    }

}
