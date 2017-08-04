using System;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MyTreeView tr;
        public Form1()
        {
            InitializeComponent();

            tr = new MyTreeView();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {            
            tr.MakeTreeView(this.treeView1);
            //AddChild child = new AddChild();
            //child.Show();
        }

        //Adds child node
        private void button2_Click(object sender, EventArgs e)
        {
            tr.AddNode(treeView1, textBox1, textBox2);
            //tr = new MyTreeView();
            tr.MakeTreeView(treeView1);
        }
        //delete child node
        private void button3_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            selectedNode.Nodes.Remove(selectedNode);
        }

        public string Txt
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public TreeView Tr
        {
            get { return this.treeView1; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tr.MakeTreeView(this.treeView1);
        }
    }
}   