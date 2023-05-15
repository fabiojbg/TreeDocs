using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeDocs.ClientApp.Model
{
    public class TreeViewNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public bool HasChildren => Children?.Any()==true ? true : false;
        public string Type => ParentId==null ? "user" : HasChildren ? "folder" : "note";
        public List<TreeViewNode> Children { get; set; }

        public override string ToString()
        {
            return $"{Id}({Name})";
        }
    }
}
