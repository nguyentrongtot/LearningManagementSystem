using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.EntityModel;

namespace PRN232.LMS.Services.Models
{
    public class StudentDTO
    {
        public int StudentId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public List<EnrollmentDTO> EnrollmentDTOs { get; set; } = new List<EnrollmentDTO>();
    }
}
