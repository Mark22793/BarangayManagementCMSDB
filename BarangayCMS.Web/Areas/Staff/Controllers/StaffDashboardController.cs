using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayCMS.DAL.Context;
using BarangayCMS.Entities;
using BarangayManagementSystem.Areas.Staff.ViewModels;

namespace BarangayCMS.Web.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff,Staff / Encoder,Admin,SuperAdmin")]
    [Route("Staff/[controller]")]
    public class StaffDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 🔑 Pinagsama sa malinis na HTTP GET route na walang duplicate/ambiguous matches
        [HttpGet]
        [Route("")]
        [Route("Index")]
        [Route("/Staff")]
        [Route("/Staff/Dashboard")]
        public async Task<IActionResult> Index()
        {
            // Kukunin ang kasalukuyang naka-login na Staff
            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["UserFullName"] = currentUser?.FullName ?? "Staff / Encoder";

            // 1. Bibilangin ang kabuuang bilang ng disasters
            int totalAlerts = await _context.Disasters.CountAsync();

            // 2. Kukunin ang pinakabagong disaster record batay sa DateCreated
            var latestDisaster = await _context.Disasters
                .OrderByDescending(d => d.DateCreated)
                .FirstOrDefaultAsync();

            string alertMessage = null;

            if (latestDisaster != null)
            {
                // 🌟 SHORT & CLEAN FORMAT (Kagaya ng Pic #2):
                // Halimbawa: "1 active emergency alert — Sunog (Fire Incident). Review immediately."
                string alertText = $"{totalAlerts} active emergency alert{(totalAlerts > 1 ? "s" : "")}";
                alertMessage = $"{alertText} — {latestDisaster.IncidentName} ({latestDisaster.DisasterType}). Review immediately.";
            }

            // Populate dashboard data mula sa Database
            var model = new DashboardViewModel
            {
                TotalResidents = await _context.Residents.CountAsync(r => r.IsResident),
                ActiveBlotters = await _context.Complaints.CountAsync(c => c.Status == "Pending"),
                PendingCertificates = await _context.Certificates.CountAsync(c => c.Status == "Pending"),
                RecentAnnouncementsCount = await _context.Announcements.CountAsync(),

                // 🌟 Ipapasa ang active disaster alert sa model
                ActiveDisasterAlert = alertMessage
            };

            return View("~/Areas/Staff/Views/StaffDashboard/Index.cshtml", model);
        }
    }
}