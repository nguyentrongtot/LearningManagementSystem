using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Business
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }

        public string SubjectCode { get; set; } = null!;

        public string SubjectName { get; set; } = null!;

        public int Credit { get; set; }

    }
}
