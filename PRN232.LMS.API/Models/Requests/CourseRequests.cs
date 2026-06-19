namespace PRN232.LMS.API.Models.Requests;

public class CourseCreateRequest
{
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
}

public class CourseUpdateRequest
{
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int SubjectId { get; set; }
}
