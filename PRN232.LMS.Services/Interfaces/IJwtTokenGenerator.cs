using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        int GetExpiresInSeconds();
    }
}
