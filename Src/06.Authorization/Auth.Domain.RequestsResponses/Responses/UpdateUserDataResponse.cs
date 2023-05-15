using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.RequestsResponses
{
    public class UpdateUserDataResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }

        public UpdateUserDataResponse(string id, string name, string email, string[] roles)
        {
            Id = id;
            Name = name;
            Email = email;
            Roles = roles;
        }
    }
}
