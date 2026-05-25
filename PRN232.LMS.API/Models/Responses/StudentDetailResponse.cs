namespace PRN232.LMS.API.Models.Responses
{
    public class StudentDetailResponse
    {
        public int StudentId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public List<EnrollmentResponse> Enrollments { get; set; } = new List<EnrollmentResponse>();
    }
}
