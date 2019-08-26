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
       private XmlDocument doc = new XmlDocument();
       private List<Node> nodes = new List<Node>();
       private List<Transition> ts = new List<Transition>();


       public FileManager(string path) {
            doc.Load(path);
       }

       public void LoadFile()
        {
        
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

        public void Unreachable()
        {
            XmlNodeList states = doc.GetElementsByTagName("state");
            XmlNodeList transitions = doc.GetElementsByTagName("transition");

            for(int i = 0; i < states.Count; i++)
            {
                for(int y = 0; y < nodes.Count; y++)//Make it a for
                {
                    if(states[i].Attributes["id"].Value == nodes[y].id)
                    {
                        for(int j = 0; j < ts.Count; j++)
                        {
                            if(states[i].Attributes["id"].Value == ts[j].to || nodes[y].initial)
                            {
                                break;
                            }
                            if (j == ts.Count - 1)
                            {
                                for(int z = 0; z < transitions.Count; z++)
                                {
                                    foreach(XmlNode x in transitions[z].SelectNodes("from"))
                                    {
                                        if(x.InnerText == states[i].Attributes["id"].Value)
                                        {
                                            transitions[z].ParentNode.RemoveChild(transitions[z]);
                                            ts.Remove(ts[z]);
                                        }
                                    }
                                }
                                states[i].ParentNode.RemoveChild(states[i]);
                                nodes.Remove(nodes[i]);
                            }
                        }
                    }
                }
            }

            doc.Save("output.jff");
        }

        public void MINIMINIMINI()
        {
            int[,] nodeArray = new int[nodes.Count, nodes.Count];

            int erase_index = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                for(int j = erase_index; j < nodes.Count; j++)
                {
                    nodeArray[i, j] = -1;
                }
                erase_index += 1;
            }
            
            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if(nodeArray[i,j] != -1)
                    {
                        nodeArray[i, j] = -2;
                    }
                }
            }

            //Paso 0
            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if((nodes[i].final && !nodes[j].final) || (!nodes[i].final && nodes[j].final))
                    {
                        if(nodeArray[i,j] == -2)
                        {
                            nodeArray[i, j] = 0;
                        }
                    }
                }
            }
            
           //Paso 1
           for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if(nodeArray[i,j] == -2)
                    {
                        Distinguible(i, j, nodeArray, 0);
                    }
                }
            }
           
            //Paso 2
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodeArray[i, j] == -2)
                    {
                        Distinguible(i, j, nodeArray, 1);
                    }
                }
            }

            //Paso 3
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodeArray[i, j] == -2)
                    {
                        Distinguible(i, j, nodeArray, 2);
                    }
                }
            }
            /*
            string cont = "";
            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    cont = cont + " " + nodeArray[i, j];
                }
                cont += "\n";
            }

            MessageBox.Show(cont);*/

            ModificarXML(nodeArray);
        }
        
        public void ModificarXML(int[,]Arr)
        {
            XmlNodeList auto = doc.GetElementsByTagName("automaton");
            XmlNodeList states = doc.GetElementsByTagName("state");

            XmlNodeList transitions = doc.GetElementsByTagName("transition");

            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if (Arr[i, j] == -2)
                    {
                        for (int c = 0; c < states.Count; c++)
                        {
                            if (!string.IsNullOrEmpty(states[c].Attributes["id"].Value))
                            {
                                if (states[c].Attributes["id"].Value == nodes[i].id || states[c].Attributes["id"].Value == nodes[j].id)
                                {
                                    states[c].ParentNode.RemoveChild(states[c]);
                                    c--;
                                }
                            }
                        }

                        XmlElement nState = doc.CreateElement("state");
                        XmlNode xVal = doc.CreateElement("x");
                        XmlNode yVal = doc.CreateElement("y");
                        XmlNode ini = doc.CreateElement("initial");
                        XmlNode fin = doc.CreateElement("final");

                        nState.SetAttribute("id", (states.Count+100).ToString());
                        nState.SetAttribute("name", "q" + nodes[i].id + "/q" + nodes[j].id);

                        xVal.InnerText = "0";
                        yVal.InnerText = "0";


                        nState.AppendChild(xVal);
                        nState.AppendChild(yVal);

                        if (nodes[i].initial || nodes[j].initial)
                        {
                            nState.AppendChild(ini);
                        }
                        if (nodes[i].final || nodes[j].final)
                        {
                            nState.AppendChild(fin);
                        }

                        auto[0].AppendChild(nState);
                    }
                }
            }

            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if(Arr[i,j] == -2)
                    {
                        string nameF = nodes[i].id;
                        string nameT = nodes[j].id;

                        for(int c = 0; c < transitions.Count; c++)
                        {
                            foreach(XmlNode t in transitions[c])
                            {
                                if(t.Name == "from")
                                {
                                    if (t.InnerText == nameF || t.InnerText == nameT)
                                    {
                                        transitions[c].ParentNode.RemoveChild(transitions[c]);
                                        c--;
                                        break;
                                    }
                                }
                                if(t.Name == "to")
                                {
                                    if (t.InnerText == nameF || t.InnerText == nameT)
                                    {
                                        transitions[c].ParentNode.RemoveChild(transitions[c]);
                                        c--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for(int i = 0; i < nodes.Count; i++)
            {
                for(int j = 0; j < nodes.Count; j++)
                {
                    if (Arr[i, j] == -2)
                    {
                        string name = nodes[i].id;
                        string name2 = nodes[j].id;

                       for(int z = 0; z < ts.Count; z++)
                       {
                            if(ts[z].from == nodes[i].id || ts[z].from == nodes[j].id)
                          {
                                XmlElement nTrans = doc.CreateElement("transition");
                                XmlNode from = doc.CreateElement("from");
                                XmlNode to = doc.CreateElement("to");
                                XmlNode read = doc.CreateElement("read");

                                for(int b = 0; b < states.Count; b++)
                                {
                                    if(states[b].Attributes["name"].Value == "q" + nodes[i].id + "/q" + nodes[j].id)
                                    {
                                        from.InnerText = states[b].Attributes["id"].Value;
                                        for(int u = 0; u < nodes.Count; u++)
                                        {
                                            for (int p = 0; p < nodes.Count; p++)
                                            {
                                                if (Arr[u, p] == -2)
                                                {
                                                    if (nodes[u].id == ts[z].to || nodes[p].id == ts[z].to)
                                                    {
                                                        foreach (XmlNode st in states)
                                                        {
                                                            if (st.Attributes["name"].Value == "q"+nodes[u].id+"/q"+nodes[p].id)
                                                            {
                                                                to.InnerText = st.Attributes["id"].Value;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ts[z].to != nodes[i].id && ts[z].to != nodes[j].id)
                                {
                                    foreach(XmlNode st in states)
                                    {
                                        if(st.Attributes["id"].Value == ts[z].to)
                                        {
                                            to.InnerText = ts[z].to;
                                            break;
                                        }
                                    }
                                }
                           

                                read.InnerText = ts[z].read;

                                nTrans.AppendChild(from);
                                nTrans.AppendChild(to);
                                nTrans.AppendChild(read);
                                if (!string.IsNullOrEmpty(to.InnerText))
                                {
                                    auto[0].AppendChild(nTrans);
                                }
                                else
                                {
                                    
                                }
                                
                          } 

                          if(ts[z].to == nodes[i].id || ts[z].to == nodes[j].id)
                            {
                                XmlElement nTrans = doc.CreateElement("transition");
                                XmlNode from = doc.CreateElement("from");
                                XmlNode to = doc.CreateElement("to");
                                XmlNode read = doc.CreateElement("read");

                                for (int b = 0; b < states.Count; b++)
                                {
                                    if (states[b].Attributes["name"].Value == "q" + nodes[i].id + "/q" + nodes[j].id)
                                    {
                                        to.InnerText = states[b].Attributes["id"].Value;
                                        if (ts[z].from == nodes[i].id || ts[z].from == nodes[j].id)
                                            for (int u = 0; u < nodes.Count; u++)
                                            {
                                                for (int p = 0; p < nodes.Count; p++)
                                                {
                                                    if (Arr[u, p] == -2)
                                                    {
                                                        if (nodes[u].id == ts[z].from || nodes[p].id == ts[z].from)
                                                        {
                                                            foreach (XmlNode st in states)
                                                            {
                                                                if (st.Attributes["name"].Value == "q" + nodes[u].id + "/q" + nodes[p].id)
                                                                {
                                                                    from.InnerText = st.Attributes["id"].Value;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                    }
                                }

                                if (ts[z].from != nodes[i].id && ts[z].from != nodes[j].id)
                                {
                                    foreach (XmlNode st in states)
                                    {
                                        if (st.Attributes["id"].Value == ts[z].from)
                                        {
                                            from.InnerText = ts[z].from;
                                            break;
                                        }
                                    }
                                }

                                read.InnerText = ts[z].read;

                                nTrans.AppendChild(from);
                                nTrans.AppendChild(to);
                                nTrans.AppendChild(read);

                                if(!string.IsNullOrEmpty(from.InnerText))
                                {
                                    auto[0].AppendChild(nTrans);
                                }
                                else
                                {
                                    
                                }
                                
                            }
                        }
                    }
                }
            }

            doc.Save("output.jff");
        }

        public void Distinguible(int i, int j, int[,]Arr, int prevNum)
        { 
            Node tmp1 = nodes[i];
            Node tmp2 = nodes[j];

            string ind1 = "";
            string ind2 = "";

            int idx = 0;
            int idx1 = 0;

            bool w = false;
            bool w1 = false;
            bool wrote = false;

            for(int x = 0; x < ts.Count; x++)
            {
                if(ts[x].from == tmp1.id)
                {
                    ind1 = ts[x].to;
                    for(int c = 0; c < ts.Count; c++)
                    {
                        string read1 = ts[x].read.Replace(",", "");
                        string read2 = ts[c].read.Replace(",", "");

                        if(ts[c].from == tmp2.id && (read1.Contains(read2) || read2.Contains(read1)))
                        {
                            ind2 = ts[c].to;

                            for (int y = 0; y < nodes.Count; y++)
                            {

                                if (nodes[y].id == ind1)
                                {
                                    idx = y;
                                    w = true;
                                }
                                if (nodes[y].id == ind2)
                                {
                                    idx1 = y;
                                    w1 = true;
                                }

                                if ((Arr[idx, idx1] == prevNum || Arr[idx1, idx] == prevNum) && (w && w1))
                                {
                                    Arr[i, j] = prevNum + 1;
                                    w = false;
                                    w1 = false;
                                    wrote = true;
                                    break;
                                }
                                if (!wrote)
                                {
                                    if(w1 && w)
                                    {
                                        w = false;
                                        w1 = false;
                                    }
                                }
                                wrote = false;              
                            }
                        }
                    }
                }
            }     
        }

    }
}
