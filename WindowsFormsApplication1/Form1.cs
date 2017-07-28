using System;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        treeview tr;
        public Form1()
        {
            InitializeComponent();

            tr = new treeview();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {            
            tr.MakeTreeView(this.treeView1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //AddChild child = new AddChild();
            //child.Show();
            tr.AddNode(textBox1.Text);
            //tr.AddTrNd(treeView1, textBox1);
            //MessageBox.Show(selectedNode.Text);
            //MessageBox.Show(tr.AddNode(selectedNode.Text));
            // MessageBox.Show(selectedNode.Parent.GetNodeCount(false).ToString());

        }

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
    }
}   