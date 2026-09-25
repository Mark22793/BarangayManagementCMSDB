using System;
using System.ComponentModel.DataAnnotations;

namespace BarangayCMS.Areas.Staff.ViewModels
{
    public class ResidentViewModel
    {
        public int ResidentId { get; set; }

        [Required(ErrorMessage = "Ang Pangalan (First Name) ay kinakailangan.")]
        [MaxLength(100, ErrorMessage = "Hindi pwedeng lumampas sa 100 karakter.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ang Apelyido (Last Name) ay kinakailangan.")]
        [MaxLength(100, ErrorMessage = "Hindi pwedeng lumampas sa 100 karakter.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Hindi pwedeng lumampas sa 100 karakter.")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ang Petsa ng Kapanganakan (Birth Date) ay kinakailangan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

        [Required(ErrorMessage = "Pumili ng Kasarian (Gender).")]
        [MaxLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "Civil Status")]
        public string CivilStatus { get; set; } = "Single";

        [MaxLength(20, ErrorMessage = "Hindi pwedeng lumampas sa 20 karakter.")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        // 🔑 NAKAHIWALAY NA PUROK / AREA
        [Required(ErrorMessage = "Pumili ng Area / Purok.")]
        [MaxLength(100)]
        [Display(Name = "Purok / Area / Sitio")]
        public string Purok { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Hindi pwedeng lumampas sa 255 karakter.")]
        [Display(Name = "Complete Address")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Hindi pwedeng lumampas sa 100 karakter.")]
        [Display(Name = "Occupation")]
        public string Occupation { get; set; } = string.Empty;

        [Display(Name = "Registered Barangay Voter?")]
        public bool IsVoter { get; set; }

        [Display(Name = "Person With Disability (PWD)")]
        public bool IsPwd { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date Registered")]
        public DateTime DateRegistered { get; set; } = DateTime.Now;

        public string FullName => $"{LastName}, {FirstName} {MiddleName}".Trim();

        // 🧮 AUTOMATIC DEMOGRAPHIC CLASSIFICATIONS (derived from BirthDate) —
        // pareho ng lohika sa Admin para iisa ang pag-uuri sa buong sistema.
        public int Age
        {
            get
            {
                if (BirthDate == default) return 0;
                var today = DateTime.Today;
                int age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age < 0 ? 0 : age;
            }
        }

        public bool IsYouth => Age >= 15 && Age <= 30;
        public bool IsSeniorCitizen => Age >= 60;

        public string AgeGroup
        {
            get
            {
                if (Age < 15) return "Child (0-14)";
                if (Age <= 30) return "Youth (15-30)";
                if (Age <= 59) return "Adult (31-59)";
                return "Senior (60+)";
            }
        }

        public string YouthClassification => IsYouth ? "Youth (SK)" : "Not Youth";
        public string SeniorClassification => IsSeniorCitizen ? "Senior Citizen" : "Not Senior";
        public string PwdClassification => IsPwd ? "PWD" : "Non-PWD";
    }
}