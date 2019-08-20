using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JflapsTool
{
    class Transition
    {
        public string from { get; set; }
        public string to { get; set; }
        public string read { get; set; }

        public Transition() { }

        public Transition(string f, string t, string r)
        {
            from = f;
            to = t;
            read = r;
        }

        public List<Transition> transitions = new List<Transition>();
    }
}
