using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace JflapsTool
{
    class FileManager
    {
       public void LoadFile(string path, List<Node> nodes, List<Transition> ts)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNodeList states = doc.GetElementsByTagName("state");
            XmlNodeList transitions = doc.GetElementsByTagName("transition");

            foreach(XmlNode node in states)
            {
                string id = node.Attributes["id"].Value;
                string name = node.Attributes["name"].Value;
                bool ini = false;
                bool fini = false;

                foreach (XmlNode fin in node)
                {
                    if(fin.Name == "initial")
                    {
                        ini = true;
                    }
                    if(fin.Name == "final")
                    {
                        fini = true;
                    }
                }

                Node n = new Node(id, name, ini, fini);
                
                nodes.Add(n);
            }

            foreach (XmlNode t in transitions)
            {
                Transition temp = new Transition();

                foreach(XmlNode at in t)
                {

                    string f = at.Name;

                    if(at.Name == "from")
                    {
                        temp.from = at.InnerText;
                        
                    }
                    else if(at.Name == "to")
                    {
                        temp.to = at.InnerText;
                        
                    }
                    else if(at.Name == "read")
                    {
                        temp.read = at.InnerText;
                        f = null;
                    }
                    if (string.IsNullOrEmpty(f))
                    {
                        ts.Add(temp);
                    }
                }
            }
        }

        public void Unreachable(List<Node> nodes, List<Transition> ts)
        {
            for(int  i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < ts.Count; j++)
                {
                    if(nodes[i].id == ts[j].to || nodes[i].initial)
                    {
                        break;
                    }
                    if(j == ts.Count - 1)
                    {
                        nodes.Remove(nodes[i]);
                    }
                }
            }
            for(int x = 0; x < nodes.Count; x++)
            {
                MessageBox.Show(nodes[x].id + " " + nodes[x].name);
            }
        }
    }
}
