using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Types;
using SqlServerTypes;
using System.Data.SqlClient;
using System.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeTreeView(this.treeView1, GetData());
        }

        public DataTable GetData()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=pdmsrv;Initial Catalog=TaskDataBase;Persist Security Info=True;User ID=airventscad;Password=1";
            conn.Open();
            string query = "SELECT ProjectHid.hid.ToString() AS hid, Projects.ProjectName FROM ProjectHid INNER JOIN Projects ON Projects.ProjectID = ProjectHid.ProjectID";

            DataTable dt = new DataTable();
            SqlCommand command = conn.CreateCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            SqlParameter par = new SqlParameter();

            par.ParameterName = "hid";
            par.Value = 30;
            par.SqlDbType = SqlDbType.Int;
            par.Direction = par.Direction;

            command.CommandType = CommandType.Text;
            command.CommandText = query;

            dt.Columns.Add("HierarchyId", typeof(SqlHierarchyId));
            dt.Columns.Add("ProjectName", typeof(string));

            da.SelectCommand = command;
            da.Fill(dt);
            foreach (DataRow oRow in dt.Rows)
            {
                oRow["ProjectName"] = oRow["ProjectName"];
                //convert the string back into a hierarchyid
                oRow["HierarchyId"] = SqlHierarchyId.Parse((string)oRow["hid"]);
            }
            return dt;
        }
        public void MakeTreeView(TreeView oTV, DataTable dt)
        {
            oTV.Nodes.Clear();
            string sKeyField = "HierarchyId";
            TreeNode oNode;
            SqlHierarchyId iID = new SqlHierarchyId();
            EnumerableRowCollection<DataRow> query2 = from TNodes in dt.AsEnumerable()
                                                      where TNodes.Field<SqlHierarchyId>(sKeyField).GetAncestor(1).Equals(iID)
                                                      select TNodes;
            DataView oDV = query2.AsDataView();
            if (oDV.Count == 1)
            {
            //load up a node
            oNode = new TreeNode(oDV[0]["ProjectName"].ToString());

            //put the datarow into the tag property
            oNode.Tag = oDV[0].Row;
            //load up the children
            LoadNodeSQLHierarchy(oNode, dt);

            //add the node hierarchy to the tree
            oTV.Nodes.Add(oNode);
            }
        }

        /// <summary>
		/// Load up the children 
		/// </summary>
		/// <param name="oParent">parent node</param>
		/// <param name="oTable">datatable with the nodekey</param>
		private void LoadNodeSQLHierarchy(TreeNode oParent, DataTable oTable)
        {
            // make sure there are no existing nodes in case this is a reload of the node
            oParent.Nodes.Clear();

            //get the nodekey from the tag property of the parent node
            SqlHierarchyId iID = new SqlHierarchyId();
            DataRow oRow = (DataRow)oParent.Tag;
            iID = (SqlHierarchyId)oRow["HierarchyId"];

            //filter the datatable on for the children
            EnumerableRowCollection<DataRow> query = from order in oTable.AsEnumerable()
                                                     where order.Field<SqlHierarchyId>("HierarchyId").GetAncestor(1).Equals(iID)
                                                     select order;

            //add the nodes to the tree
            DataView oDV = query.AsDataView();
            foreach (DataRowView oDR in oDV)
            {
                TreeNode oNode = new TreeNode(oDR["ProjectName"].ToString());
                oNode.Tag = oDR.Row;

                LoadNodeSQLHierarchy(oNode, oTable);
                oParent.Nodes.Add(oNode);
            }
        }
    }
}   