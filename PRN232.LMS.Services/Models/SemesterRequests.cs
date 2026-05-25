using System;

namespace PRN232.LMS.Services.Models
{
    public class SemesterCreateRequest
    {
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SemesterUpdateRequest
    {
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
