using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN232.LMS.Services.Business;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthTokenDTO> LoginAsync(LoginRequest request);
        Task<AuthTokenDTO> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
