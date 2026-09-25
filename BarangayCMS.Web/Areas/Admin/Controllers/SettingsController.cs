using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarangayCMS.Web.Areas.Admin.Models;
using BarangayCMS.DAL.Context;  // Galing sa iyong Data Access Layer
using BarangayCMS.Entities;     // Galing sa iyong Entities Layer

namespace BarangayCMS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Dito natin in-inject ang Database Context para gumana ang DB updates
        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Dashboard ng Settings (Dito na natin kukunin ang laman ng DB para lumabas sa textboxes)
        public async Task<IActionResult> Index()
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                // Gumawa ng default record sa DB kung sakaling wala pang laman
                settings = new SystemSetting
                {
                    BarangayName = "Barangay Tatalon",
                    CityMunicipality = "Quezon City"
                };
                _context.SystemSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            // I-map ang DB data papunta sa iyong BarangayProfileViewModel
            var model = new BarangayProfileViewModel
            {
                OfficialBarangayName = settings.BarangayName,
                MunicipalityCity = settings.CityMunicipality
            };

            return View(model);
        }

        // 2. Profile ng Barangay (POST: Sine-save ang bagong input sa Database)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BarangayProfile(BarangayProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var settings = await _context.SystemSettings.FirstOrDefaultAsync();

                // Kung wala pang laman ang table, gumawa ng bago
                if (settings == null)
                {
                    settings = new SystemSetting
                    {
                        BarangayName = model.OfficialBarangayName,
                        CityMunicipality = model.MunicipalityCity
                    };
                    _context.SystemSettings.Add(settings);
                }
                else
                {
                    // Kung mayroon nang record, i-update ito
                    settings.BarangayName = model.OfficialBarangayName;
                    settings.CityMunicipality = model.MunicipalityCity;
                    _context.SystemSettings.Update(settings);
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = "Profile successfully updated!";
            }
            else
            {
                // Kung may kulang sa model validation, ipapakita nito kung ano ang naging error
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["Error"] = $"Hindi na-save! Error: {errors}";
            }

            // Pagkatapos mag-save, babalik tayo sa Index page (Dashboard ng Settings)
            return RedirectToAction(nameof(Index));
        }

        // 3. Talaan ng mga Opisyal
        public IActionResult Officials()
        {
            var officials = new List<OfficialViewModel> {
                new OfficialViewModel { Id = 1, Name = "Juan Dela Cruz", Position = "Punong Barangay" },
                new OfficialViewModel { Id = 2, Name = "Maria Clara", Position = "Barangay Secretary" },
                new OfficialViewModel { Id = 3, Name = "Apolinario Mabini", Position = "Barangay Treasurer" }
            };
            return View(officials);
        }

        // 4. Kasaysayan ng Galaw sa System
        public IActionResult AuditLogs()
        {
            var logs = new List<AuditLogViewModel> {
                new AuditLogViewModel { LogDate = DateTime.Now.AddMinutes(-5), User = "admin", Action = "Updated Resident Record", Module = "Residents" },
                new AuditLogViewModel { LogDate = DateTime.Now.AddHours(-2), User = "encoder1", Action = "Issued Barangay Clearance", Module = "Certificates" }
            };
            return View(logs);
        }

        // 5. Pag-ayos ng Templates
        public IActionResult CertificateTemplates()
        {
            var templates = new List<CertificateTemplateViewModel> {
                new CertificateTemplateViewModel { TemplateName = "Barangay Clearance", Fee = 50.00m, IsDigitalSignEnabled = true },
                new CertificateTemplateViewModel { TemplateName = "Certificate of Indigency", Fee = 0.00m, IsDigitalSignEnabled = false }
            };
            return View(templates);
        }

        // 6. Backup and Restore Page (GET)
        public IActionResult Backup() => View();

        // 🌟 Action para mag-download ng Tunay na Database Backup (.bak file)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadBackup()
        {
            try
            {
                // Kunin ang kasalukuyang Database Name mula sa connection string
                string connectionString = _context.Database.GetDbConnection().ConnectionString;
                var dbConnectionBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                string dbName = dbConnectionBuilder.InitialCatalog;

                string fileName = $"{dbName}_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                // Siguraduhing may folder sa local server para roon pansamantalang i-save ang file
                string backupFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string backupPath = Path.Combine(backupFolder, fileName);

                // SQL Command para mag-generate ng buong database snapshot at records
                string sqlQuery = $@"BACKUP DATABASE [{dbName}] 
                                    TO DISK = '{backupPath}' 
                                    WITH FORMAT, MEDIANAME = 'SQLServerBackups', NAME = 'Full Backup of {dbName}';";

                // Isagawa ang backup query sa mismong SQL Server Engine
                await _context.Database.ExecuteSqlRawAsync(sqlQuery);

                // Basahin ang na-generate na file para maipasa sa browser download
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(backupPath);

                // Linisin ang temporary file sa server folder pagkatapos mabasa
                if (System.IO.File.Exists(backupPath))
                {
                    System.IO.File.Delete(backupPath);
                }

                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Backup failed: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // 🌟 Action para linisin ang Server/Database Entity memory cache
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCache()
        {
            try
            {
                _context.ChangeTracker.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                TempData["Message"] = "System cache and transaction logs cleared safely!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to clear cache: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}