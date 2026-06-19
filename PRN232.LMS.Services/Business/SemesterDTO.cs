using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Business
{
    public class SemesterDTO
    {
        public int SemesterId { get; set; }

        public string SemesterName { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
