using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Entities
{
    public class RefreshToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }

        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public User User { get; set; } = null!;
    }
}
