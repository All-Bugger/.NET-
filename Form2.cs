using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;


namespace termFinal
{
    public partial class Form2 : Form
    {
        // opera 代表哪钟操作，0是删除，1是增加
        public delegate void TypesChange(int opera,string type);
        public event TypesChange typesChange;

        SQLiteConnection conn;
        public Form2()
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
            loadTypes();
        }

        private void loadTypes()
        {
            conn.Open();
            string sql = "select * from types";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql,conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            listView1.Items.Clear();
            foreach(DataRow row in dt.Rows)
            {
                listView1.Items.Add(row[0].ToString());
            }
            conn.Close();
        }
        
        // 添加
        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();
            string newType = textBox1.Text;
            // 看有没有重复
            for(int i = 0; i<listView1.Items.Count;i++)
            {
                if (newType.Equals(listView1.Items[i].Text))
                {
                    return;
                }
            }
            string sql = "insert into types(typename) values('" + newType + "');";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            loadTypes();
            typesChange.Invoke(1,newType);
        }

        // 删除
        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            string typeDel = textBox1.Text;
            string sql = "delete from types where typename = '" + typeDel + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            loadTypes();
            typesChange.Invoke(0,typeDel);
        }

        // 选中后，将内容显示到下面框里
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i = 0; i<listView1.SelectedItems.Count;i++)
            {
                textBox1.Text = listView1.SelectedItems[i].Text;
                break;
            }
        }
    }
}
