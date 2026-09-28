using System.Collections.Generic;

namespace BarangayManagementSystem.Areas.Staff.ViewModels
{
    public class DashboardViewModel
    {
        // STATS COMMAND CORES
        public int TotalResidents { get; set; }
        public int PendingCertificates { get; set; }
        public int ActiveBlotters { get; set; }
        public int RecentAnnouncementsCount { get; set; }

        // 🌟 Idinagdag para sa Disaster / Emergency Alert Banner
        public string ActiveDisasterAlert { get; set; }

        // RECENT DATA ARRAYS (Para may mag-load na data sa dashboard lists)
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new List<RecentActivityViewModel>();
    }

    public class RecentActivityViewModel
    {
        public string ActivityType { get; set; } = string.Empty; // e.g., "Resident", "Certificate", "Blotter"
        public string Description { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}