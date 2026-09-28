using System;
using System.Collections.Generic;

namespace BarangayManagementSystem.Areas.Admin.Models
{
    // 🌟 BAGONG DAGDAG: Para sa Main Admin Dashboard Cards
    public class AdminDashboardViewModel
    {
        public int TotalResidents { get; set; }
        public int PendingComplaints { get; set; }
        public int CertificatesHandled { get; set; }
        public int UnreadMessages { get; set; }
        public string SystemStatus { get; set; } = "Operational";

        public string ActiveDisasterAlert { get; set; }
    }

    // Para sa Residents Page
    public class DashboardResidentViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Birthdate { get; set; } = string.Empty;
        public string CivilStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // Para sa Complaints Page
    public class DashboardComplaintViewModel
    {
        public string CaseId { get; set; } = string.Empty;
        public string Complaint { get; set; } = string.Empty;
        public string Complainant { get; set; } = string.Empty;
        public string Respondent { get; set; } = string.Empty;
        public string AssignedOfficer { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // Para sa Certificates Page
    public class DashboardCertificateViewModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Fee { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}