namespace ShiftApplication.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using ShiftApplication.Models;

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // Shift Handover Tables
        // =========================
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<AccidentLog> AccidentLogs { get; set; }
        public DbSet<IncidentLog> IncidentLogs { get; set; }
        public DbSet<ManpowerLog> ManpowerLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Shift → IdentityUser (Supervisor)
            builder.Entity<Shift>()
                .HasOne(s => s.Supervisor)
                .WithMany()
                .HasForeignKey(s => s.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shift → Accident Logs
            builder.Entity<AccidentLog>()
                .HasOne(a => a.Shift)
                .WithMany(s => s.Accidents)
                .HasForeignKey(a => a.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shift → Incident Logs
            builder.Entity<IncidentLog>()
                .HasOne(i => i.Shift)
                .WithMany(s => s.Incidents)
                .HasForeignKey(i => i.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shift → Manpower Logs
            builder.Entity<ManpowerLog>()
                .HasOne(m => m.Shift)
                .WithMany(s => s.ManpowerDetails)
                .HasForeignKey(m => m.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
