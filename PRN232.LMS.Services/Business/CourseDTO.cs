using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Business
{
    public class CourseDTO
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; } = null!;

        public int SemesterId { get; set; }

        public int SubjectId { get; set; }
        public  SemesterDTO? SemesterDTO { get; set; } = new SemesterDTO();
        public SubjectDTO? SubjectDTO { get; set; } = new SubjectDTO();
    }
}
