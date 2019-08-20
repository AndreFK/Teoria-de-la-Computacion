using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JflapsTool
{
    class Node
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool initial { get; set; }
        public bool final { get; set; }

        public Node()
        {
            id = "";
            name = "";
            initial = false;
            final = false;
        }

        public Node(string Id, string Name, bool i, bool f)
        {
            id = Id;
            name = Name;
            initial = i;
            final = f;
        }

        public List<Node> nodes = new List<Node>();
    }
}
