using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftApplication.Data;
using ShiftApplication.Models;
using ShiftApplication.ViewModels;
using System.Reflection.Metadata;
 

namespace ShiftApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts
            .Include(s => s.Supervisor)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

            return View(shifts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateShiftViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShiftViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var shift = new Shift
            {
                StartTime = model.StartTime,
                IsClaimed = false,
                IsClosed = false
            };
            var user = await _userManager.GetUserAsync(User);

            shift.SupervisorId = user.Id;
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Claim(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);

            if (shift == null)
                return NotFound();

            if (shift.IsClaimed)
                return BadRequest("Shift already claimed.");

            return View(new ShiftClaimViewModel
            {
                ShiftId = shift.Id,
                StartTime = shift.StartTime
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Claim(ShiftClaimViewModel model)
        {
            var shift = await _context.Shifts.FindAsync(model.ShiftId);

            if (shift == null)
                return NotFound();

            if (shift.IsClaimed)
                return BadRequest("Shift already claimed.");

            var user = await _userManager.GetUserAsync(User);

            shift.SupervisorId = user.Id;
            shift.IsClaimed = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Log(int shiftId)
        {
            var shift = await _context.Shifts.FindAsync(shiftId);

            if (shift == null)
                return NotFound();

            if (shift.IsClosed)
                return BadRequest("Shift is closed.");

            return View(new ShiftLogViewModel
            {
                ShiftId = shiftId
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Log(ShiftLogViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var shift = await _context.Shifts.FindAsync(model.ShiftId);

            if (shift == null)
                return NotFound();

            if (shift.IsClosed)
                return BadRequest("Shift is closed.");

            switch (model.LogType)
            {
                case LogType.Accident:
                    _context.AccidentLogs.Add(new AccidentLog
                    {
                        ShiftId = model.ShiftId,
                        DateTime = model.DateTime,
                        Description = model.Description
                    });
                    break;

                case LogType.Incident:
                    _context.IncidentLogs.Add(new IncidentLog
                    {
                        ShiftId = model.ShiftId,
                        DateTime = model.DateTime,
                        Description = model.Description
                    });
                    break;

                case LogType.Manpower:
                    _context.ManpowerLogs.Add(new ManpowerLog
                    {
                        ShiftId = model.ShiftId,
                        DateTime = model.DateTime,
                        Description = model.Description
                    });
                    break;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);

            if (shift == null)
                return NotFound();

            if (shift.IsClosed)
                return BadRequest("Shift already closed.");

            shift.IsClosed = true;
            shift.EndTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Report(int id)
        {
            var shift = await _context.Shifts
                .Include(s => s.Supervisor)
                .Include(s => s.Accidents)
                .Include(s => s.Incidents)
                .Include(s => s.ManpowerDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shift == null)
                return NotFound();

            var vm = new ShiftReportViewModel
            {
                ShiftId = shift.Id,
                SupervisorName = shift.Supervisor?.UserName,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Accidents = shift.Accidents
                    .Select(a => new LogItemViewModel
                    {
                        DateTime = a.DateTime,
                        Description = a.Description
                    }).ToList(),
                Incidents = shift.Incidents
                    .Select(i => new LogItemViewModel
                    {
                        DateTime = i.DateTime,
                        Description = i.Description
                    }).ToList(),
                ManpowerDetails = shift.ManpowerDetails
                    .Select(m => new LogItemViewModel
                    {
                        DateTime = m.DateTime,
                        Description = m.Description
                    }).ToList()
            };

            return View(vm);
        }



        public IActionResult DownloadReport(long id)
        {
            var shift = _context.Shifts
                .Include(s => s.Supervisor)
                .Include(s => s.Accidents)
                .Include(s => s.Incidents)
                .Include(s => s.ManpowerDetails)
                .FirstOrDefault(s => s.Id == id);

            if (shift == null)
                return NotFound();

            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);
            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            // Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            // Title
            document.Add(new Paragraph($"Shift Report - {shift.StartTime:dd MMM yyyy}", titleFont));
            document.Add(new Paragraph("\n"));

            // Summary
            document.Add(new Paragraph($"Supervisor: {shift.Supervisor?.UserName ?? "-"}", normalFont));
            document.Add(new Paragraph($"Status: {(shift.IsClosed ? "Closed" : shift.IsClaimed ? "Claimed" : "Open")}", normalFont));
            document.Add(new Paragraph("\n"));

            void AddLogSection(string sectionName, IEnumerable<dynamic> logs)
            {
                document.Add(new Paragraph(sectionName, headerFont));
                document.Add(new Paragraph("\n"));

                if (logs.Any())
                {
                    var table = new PdfPTable(2) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 25, 75 }); 

                    table.AddCell(new PdfPCell(new Phrase("Time", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    table.AddCell(new PdfPCell(new Phrase("Description", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    foreach (var log in logs)
                    {
                        table.AddCell(new PdfPCell(new Phrase($"{log.DateTime:dd MMM yyyy, hh:mm tt}", normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(log.Description, normalFont)));
                    }

                    document.Add(table);
                }
                else
                {
                    document.Add(new Paragraph($"No {sectionName.ToLower()} recorded", normalFont));
                }

                document.Add(new Paragraph("\n"));
            }

            // Accidents
            AddLogSection("Accidents", shift.Accidents);

            // Incidents
            AddLogSection("Incidents", shift.Incidents);

            // Manpower
            AddLogSection("Manpower Details", shift.ManpowerDetails);

            document.Close();
            var pdfBytes = ms.ToArray();

            return File(pdfBytes, "application/pdf", $"ShiftReport_{shift.Id}.pdf");
        }

    }
}
