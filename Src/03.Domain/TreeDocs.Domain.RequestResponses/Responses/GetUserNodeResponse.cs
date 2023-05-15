using System;
using System.Collections.Generic;
using System.Text;
using TreeDocs.Domain.Entities;
using TreeDocs.Domain.Enums;

namespace TreeDocs.Domain.RequestsResponses
{
    public class GetUserNodeResponse
    {
        public string OwnerId { get; set; }
        public Node Node { get; set; }
    }
}
