using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JflapsTool
{
    public partial class JflapTool : Form
    {
        FileManager fm = new FileManager();
        List<Node> nodes = new List<Node>();
        List<Transition> ts = new List<Transition>();

        public JflapTool()
        {
            InitializeComponent();
        }

        private void load_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Elegir Archivo",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "jff",
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if(of.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = of.FileName;
            }
        }

        private void min_btn_Click(object sender, EventArgs e)
        {
            fm.LoadFile(textBox1.Text, nodes, ts);
            fm.Unreachable(nodes, ts);
        }
    }
}
