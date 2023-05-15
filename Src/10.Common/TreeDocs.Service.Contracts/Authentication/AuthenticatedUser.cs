using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeDocs.Service.Contracts.Authentication
{
    public class AuthenticatedUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
