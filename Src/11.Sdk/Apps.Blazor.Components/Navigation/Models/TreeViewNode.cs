using System;
using System.Collections.Generic;
using System.Text;

namespace Apps.Blazor.Components.Navigation
{
    public class TreeViewNode
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool open { get; set; } = false;
        public string parentId { get; set; }
        public List<TreeViewNode> children { get; set; } = new List<TreeViewNode>();
    }

}
