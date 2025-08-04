using System;
using System.Collections.Generic;
using System.Text;
using TreeNotes.Domain.Entities;
using TreeNotes.Domain.Enums;

namespace TreeNotes.Domain.RequestsResponses
{
    public class GetUserNodeResponse
    {
        public string OwnerId { get; set; }
        public Node Node { get; set; }
    }
}
