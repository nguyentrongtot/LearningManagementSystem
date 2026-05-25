using System;

namespace PRN232.LMS.Services.Models
{
    public class SubjectCreateRequest
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credit { get; set; }
    }

    public class SubjectUpdateRequest
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credit { get; set; }
    }
}
