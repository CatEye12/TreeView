using Microsoft.SqlServer.Types;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class MyTreeView
    {
        DataTable dt;
        SqlDataAdapter da;
        SqlConnection conn;
        public MyTreeView()
        {
            da = new SqlDataAdapter();
            conn = new SqlConnection();
            conn.ConnectionString = "Data Source=pdmsrv;Initial Catalog=TaskDataBase;Persist Security Info=True;User ID=airventscad;Password=1";
            conn.Open();
        }
                
        private DataTable GetData()
        {
            dt = new DataTable();
            string query = "SELECT PH.hid.ToString() AS hid, P.ProjectName, P.ProjectID "
                            + "FROM ProjectHid "
                            + "PH INNER JOIN Projects P ON P.ProjectID = PH.ProjectID";
            

            SqlCommand command = conn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            dt.Columns.Add("HierarchyId", typeof(SqlHierarchyId));
            dt.Columns.Add("ProjectName", typeof(string));
            dt.Columns.Add("ProjectID", typeof(int));

            da.SelectCommand = command;
            da.Fill(dt);
            foreach (DataRow oRow in dt.Rows)
            {
                oRow["ProjectID"] = oRow["ProjectID"];
                oRow["ProjectName"] = oRow["ProjectName"];
                //convert the string back into a hierarchyid
                oRow["HierarchyId"] = SqlHierarchyId.Parse((string)oRow["hid"]);
            }
            return dt;
        }
        public void MakeTreeView(TreeView oTV)
        {
            GetData();
            //MessageBox.Show("Nodes quantity = " + oTV.Nodes.Count.ToString());
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
                oNode.Expand();

                //load up the children
                LoadNodeSQLHierarchy(oNode, dt);

                //add the node hierarchy to the tree
                oTV.Nodes.Add(oNode);
            }
            //MessageBox.Show("ODV quantity = " + oDV.Count.ToString());
        }
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

        public void AddNode(TreeView treeView, TextBox txt, TextBox txtNumber)
        {
            TreeNode selectedNode = treeView.SelectedNode;
            SqlDataReader reader;
            SqlCommand command = new SqlCommand("AddNewNodes", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@parent_name", selectedNode.Text);
            command.Parameters.AddWithValue("@new_child_name", txt.Text);
            command.Parameters.AddWithValue("@number", txtNumber.Text);
            command.Parameters.Add("@result", SqlDbType.Bit);
            command.Parameters["@result"].Direction = ParameterDirection.Output;

            reader = command.ExecuteReader();

            //int flag = 1;
            //flag = (int)command.Parameters["@result"].Value;

            //if (flag == 0)
            //{
            //    MessageBox.Show("The node with such name already excist. Pick up another name.");
            //}
            reader.Close();
            da.Fill(dt);
        }
    }
}