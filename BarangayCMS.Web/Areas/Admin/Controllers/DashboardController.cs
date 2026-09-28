using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayCMS.DAL.Context;
using BarangayCMS.BLL.Interfaces;
using BarangayManagementSystem.Areas.Admin.Models;

namespace BarangayManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IContactMessageService _contactMessages;

        public DashboardController(ApplicationDbContext context, IContactMessageService contactMessages)
        {
            _context = context;
            _contactMessages = contactMessages;
        }

        // 🌟 REAL-TIME DATABASE COUNTING WITH SHORT DISASTER ALERT BANNER
        public async Task<IActionResult> Index()
        {
            // 1. Bibilangin ang kabuuang active/registered disasters
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

            var model = new AdminDashboardViewModel
            {
                TotalResidents = await _context.Residents.CountAsync(),
                PendingComplaints = await _context.Complaints.CountAsync(c => c.Status == "Pending"),
                CertificatesHandled = await _context.Certificates.CountAsync(),
                UnreadMessages = await _contactMessages.GetUnreadCountAsync(),
                SystemStatus = "Operational",

                // Maikling alert message
                ActiveDisasterAlert = alertMessage
            };

            return View(model);
        }

        public IActionResult Residents()
        {
            return View("Resident");
        }

        public IActionResult StaffProfile()
        {
            return View();
        }
    }
}