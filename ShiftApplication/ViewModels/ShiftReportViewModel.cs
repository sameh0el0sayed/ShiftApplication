namespace ShiftApplication.ViewModels
{
    public class ShiftReportViewModel
    {
        public int ShiftId { get; set; }

        public string SupervisorName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public List<LogItemViewModel> Accidents { get; set; } = new();
        public List<LogItemViewModel> Incidents { get; set; } = new();
        public List<LogItemViewModel> ManpowerDetails { get; set; } = new();
    }

    public class LogItemViewModel
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }
}
