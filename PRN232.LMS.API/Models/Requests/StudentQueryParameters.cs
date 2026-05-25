namespace PRN232.LMS.API.Models.Requests
{
    public class StudentQueryParameters
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }
        public string? Fields { get; set; }
        public string? Expand { get; set; }
    }
}
