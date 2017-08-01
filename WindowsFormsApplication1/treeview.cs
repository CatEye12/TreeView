using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class treeview
    {
        DataTable dt;
        SqlDataAdapter da;
        SqlConnection conn;
        public treeview()
        {
            dt = new DataTable();

            conn = new SqlConnection();
            conn.ConnectionString = "Data Source=pdmsrv;Initial Catalog=TaskDataBase;Persist Security Info=True;User ID=airventscad;Password=1";
            conn.Open();
            GetData();
        }
                
        private DataTable GetData()
        {
            //SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = "Data Source=pdmsrv;Initial Catalog=TaskDataBase;Persist Security Info=True;User ID=airventscad;Password=1";
            //conn.Open();
            string query = "SELECT PH.hid.ToString() AS hid, P.ProjectName, P.ProjectID "
                            + "FROM ProjectHid "
                            + "PH INNER JOIN Projects P ON P.ProjectID = PH.ProjectID";

            
            SqlCommand command = conn.CreateCommand();
            da = new SqlDataAdapter();
            SqlParameter par = new SqlParameter();

            par.ParameterName = "hid";
            par.Value = 30;
            par.SqlDbType = SqlDbType.Int;
            par.Direction = par.Direction;

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

        public void  AddNode(TreeView treeView, TextBox txt)
        {
            #region
            //var results = from myRow in dt.AsEnumerable()
            //              where myRow.Field<string>("ProjectName") == nodeName
            //              select myRow;

            //var query2 = from TNodes in dt.AsEnumerable()
            //             where TNodes.Field<string>("ProjectID").Equals(1)
            //             select TNodes;

            //DataView oDV = query2.AsDataView();

            //string rr = null;

            //if (oDV.Count == 1)
            //{
            //    rr =  oDV[0]["ProjectID"].ToString();
            //}
            //return rr;
            #endregion

            #region 31.07
            //TreeNode selectedNode = treeView.SelectedNode;
            //TreeNode newNode;

            //SqlHierarchyId childNull = new SqlHierarchyId();
            //SqlHierarchyId newChild;
            //MessageBox.Show(dt.Rows.Count.ToString()); 
            //MessageBox.Show(txt.Text);
            //EnumerableRowCollection parentHid = from rows in dt.AsEnumerable()
            //                where rows.Field<string>("ProjectName").Equals(selectedNode.Text)
            //                select rows;

            //EnumerableRowCollection<SqlHierarchyId> childHid = from rows in dt.AsEnumerable()
            //                where rows.Field<SqlHierarchyId>("ProjectHid").Equals(parentHid)
            //                select rows.Field<SqlHierarchyId>("hid");
            
            //SqlCommand command = new SqlCommand();
            //command.CommandType = CommandType.Text;
            
            //foreach (var item in parentHid)
            //{
            //    //add first child node
            //    if (selectedNode.GetNodeCount(false) == 0)
            //    {
            //      //  newChild = item.GetDescendant(childNull, childNull);

            //        command.CommandText = "INSERT INTO Employees VALUES(" + newChild + ", " + txt.Text + ")";
            //        da.SelectCommand = command;
            //        da.Fill(dt);
            //    }
            //    //add other nodes after first
            //    else
            //    {
            //        //newChild = item.GetDescendant(childHid, childNull);
            //        //da.SelectCommand = command;
            //        //command.CommandText = "INSERT INTO Employees VALUES(" + newChild + ", " + txt.Text + ")";
            //    }
            //}
            #endregion

            TreeNode selectedNode = treeView.SelectedNode;
            SqlDataReader reader;

            // SqlCommand command2 = new SqlCommand("AddFirstChildNode", conn);
            SqlCommand command2 = new SqlCommand("GetChildHid", conn);
            
            command2.CommandType = CommandType.StoredProcedure;
            command2.Parameters.AddWithValue("@parent_name", selectedNode.Text);
            command2.Parameters.AddWithValue("@new_child_name", txt.Text);

            reader = command2.ExecuteReader();
            
            //while (reader.Read())
            //{
            //    string name = reader.GetString(reader.GetOrdinal("name"));
            //    MessageBox.Show(name);
            //}
            reader.Close();
            da.Fill(dt);
        }
    }
}