namespace PRN232.LMS.API.Models.Responses
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }  

        public int CourseId { get; set; }

        public DateTime EnrollDate { get; set; }

        public string Status { get; set; } = null!;

        public CourseResponse? Course { get; set; }
        public StudentResponse? Student { get; set; }
    }
}
