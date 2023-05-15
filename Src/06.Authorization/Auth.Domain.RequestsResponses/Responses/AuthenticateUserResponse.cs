using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.RequestsResponses
{
    public class AuthenticateUserResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }

        public AuthenticateUserResponse(string id, string name, string email, List<string> roles)
        {
            Id = id;
            Name = name;
            Email = email;
            Roles = roles;
        }
    }
}
