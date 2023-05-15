using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeDocs.ClientApp.Model
{
    public class NodeChangingEvent
    {
        public string PreviousNodeId { get; set; }
        public string NewNodeId { get; set; }
        public bool? Cancel { get; set; }
    }

    public class NodeChangedEvent
    {
        public string PreviousNodeId { get; set; }
        public string NewNodeId { get; set; }
    }
}
