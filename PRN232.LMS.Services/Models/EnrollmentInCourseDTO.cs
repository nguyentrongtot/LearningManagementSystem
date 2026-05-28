using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Models
{
    public class EnrollmentInCourseDTO
    {
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrollDate { get; set; }

        public string Status { get; set; } = null!;
        //public CourseDTO? CourseDTO { get; set; } = new CourseDTO();
        public StudentDTO? StudentDTO { get; set; } = new StudentDTO();
    }
}
