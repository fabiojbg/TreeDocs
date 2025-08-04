using System.Collections.Generic;
using System.Text;

namespace TreeNotes.Domain.RequestsResponses
{

    public class GetUserNodesResponse
    {
        public string OwnerId { get; set; }
        public IEnumerable<Node> Nodes { get; set; }
    }
}
