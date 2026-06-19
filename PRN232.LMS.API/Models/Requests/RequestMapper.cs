using Business = PRN232.LMS.Services.Business;

namespace PRN232.LMS.API.Models.Requests;

public static class RequestMapper
{
    public static Business.StudentCreateRequest ToBusiness(StudentCreateRequest request) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        DateOfBirth = request.DateOfBirth
    };

    public static Business.StudentUpdateRequest ToBusiness(StudentUpdateRequest request) => new()
    {
        FullName = request.FullName,
        Email = request.Email,
        DateOfBirth = request.DateOfBirth
    };

    public static Business.EnrollmentCreateRequest ToBusiness(EnrollmentCreateRequest request) => new()
    {
        StudentId = request.StudentId,
        CourseId = request.CourseId,
        EnrollDate = request.EnrollDate,
        Status = request.Status
    };

    public static Business.EnrollmentUpdateRequest ToBusiness(EnrollmentUpdateRequest request) => new()
    {
        StudentId = request.StudentId,
        CourseId = request.CourseId,
        EnrollDate = request.EnrollDate,
        Status = request.Status
    };

    public static Business.CourseCreateRequest ToBusiness(CourseCreateRequest request) => new()
    {
        CourseName = request.CourseName,
        SemesterId = request.SemesterId,
        SubjectId = request.SubjectId
    };

    public static Business.CourseUpdateRequest ToBusiness(CourseUpdateRequest request) => new()
    {
        CourseName = request.CourseName,
        SemesterId = request.SemesterId,
        SubjectId = request.SubjectId
    };

    public static Business.SemesterCreateRequest ToBusiness(SemesterCreateRequest request) => new()
    {
        SemesterName = request.SemesterName,
        StartDate = request.StartDate,
        EndDate = request.EndDate
    };

    public static Business.SemesterUpdateRequest ToBusiness(SemesterUpdateRequest request) => new()
    {
        SemesterName = request.SemesterName,
        StartDate = request.StartDate,
        EndDate = request.EndDate
    };

    public static Business.SubjectCreateRequest ToBusiness(SubjectCreateRequest request) => new()
    {
        SubjectCode = request.SubjectCode,
        SubjectName = request.SubjectName,
        Credit = request.Credit
    };

    public static Business.SubjectUpdateRequest ToBusiness(SubjectUpdateRequest request) => new()
    {
        SubjectCode = request.SubjectCode,
        SubjectName = request.SubjectName,
        Credit = request.Credit
    };
}
