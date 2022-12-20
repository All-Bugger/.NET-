using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace termFinal
{

    public partial class Form1 : Form
    {
        Speaker speaker;
        ArrayList events;
        SQLiteConnection conn;
        SQLiteDataAdapter adapter;
        public Form1()
        {
            InitializeComponent();
            conn = getDBConn();
            events = new ArrayList();
            speaker = new Speaker();
            loadTypes(); // 加载时间类别
            loadEvents(); // 加载数据
        }

        private SQLiteConnection getDBConn()
        {
            return new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
        }


        // 初始化数据，测试用
        private void initData()
        {
            conn.Open();
            string sql = "insert into events(title,type, time, state) values('.net作业', '工作类', 1490782464000,0)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        // 加载数据库中的数据
        private void loadEvents()
        {
            conn.Open();
            adapter = new SQLiteDataAdapter("select * from events", conn);
            conn.Close();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.Rows.Clear();
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                dataGridView1.Rows.Add(dr);
            }
            events.Clear();
            foreach (DataRow row in dt.Rows)
            {
                long time = (long)row[2];
                // 把long类型转换为DateTime类型。
                DateTime dateTime = Util.ConvertLongToDateTime(time);

                string title = (string)row[0];
                string type = (string)row[1];
                string timeStr = string.Format("{0:yyyy/MM/dd hh:mm}", dateTime);
                Int64 state = (Int64)row[3];

                dataGridView1.Rows[index].Cells[0].Value = title;
                dataGridView1.Rows[index].Cells[1].Value = type;
                dataGridView1.Rows[index].Cells[2].Value = timeStr;
                dataGridView1.Rows[index].Cells[3].Value = row[3];
                index++;
                
                events.Add(new Event(title, type, dateTime, (int)state));
            }
        }


        // 加载类型
        private void loadTypes()
        {
            conn.Open();
            adapter = new SQLiteDataAdapter("select * from types",conn);
            conn.Close();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            comboBox1.Items.Clear();
            foreach(DataRow row in dt.Rows)
            {
                comboBox1.Items.Add(row[0]);
            }
        }

        private void delEvent(string title,string type)
        {
            if(title.Equals("")|| type.Equals(""))
            {
                return;
            }
            string sql = string.Format("delete from events where title = '{0}' and type = '{1}'", title, type);
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            loadEvents();
        }

        // 实现查找功能
        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text;
            string type = comboBox1.Text;
            // 如果title为空，按类型查找
            string sql;
            if (title.Equals(""))
            {
                sql = string.Format("select * from events where type = '{0}'", type);
            }
            // 如果type 为空， 按名字查找
            else if (type.Equals(""))
            {
                sql = string.Format("select * from events where title like '%{0}%'", title);
            }
            // 都不空，结合两个条件一起查
            else
            {
                sql = string.Format("select * from events where title like '%{0}%' and type = '{1}'", title, type);
            }
            conn.Open();
            adapter = new SQLiteDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.Rows.Clear();
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                dataGridView1.Rows.Add(dr);
            }
            foreach (DataRow row in dt.Rows)
            {
                long time = (long)row[2];
                // 把long类型转换为DateTime类型。
                DateTime dateTime = Util.ConvertLongToDateTime(time);
                dataGridView1.Rows[index].Cells[0].Value = row[0];
                dataGridView1.Rows[index].Cells[1].Value = row[1];
                dataGridView1.Rows[index].Cells[2].Value = string.Format("{0:yyyy/MM/dd hh:mm}", dateTime);
                dataGridView1.Rows[index].Cells[3].Value = row[3];
                index++;
            }
            conn.Close();
        }

        // 删除选中记录
        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i<dataGridView1.SelectedRows.Count;i++)
            {
                delEvent((string)dataGridView1.SelectedRows[i].Cells[0].Value, (string)dataGridView1.SelectedRows[i].Cells[1].Value);
                break;
            }
        }

        // 新增记录
        private void button3_Click(object sender, EventArgs e)
        {
            conn.Open();
            string newTitle = textBox1.Text;
            string newType = comboBox1.Text;
            if (newTitle.Equals("") || newType.Equals(""))
            {
                MessageBox.Show("标题和内容不能为空");
                conn.Close();
                return;
            }
            DateTime dateTime = dateTimePicker1.Value;
            long newTime = Util.ConvertDateTimeToLong(dateTime);
            string state = "0";
            string sql = string.Format("insert into events(title,type,time,state) values('{0}','{1}',{2},{3})",
                newTitle,newType,newTime,state);
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            loadEvents();
        }


        // 弹出设置分类管理的页面
        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            // 事件委托完成消息传递
            form2.typesChange += new Form2.TypesChange(typeChange);
            form2.ShowDialog();
        }

        public void typeChange(int opera ,string type)
        {
            if(opera == 0)
            {
                comboBox1.Items.Remove(type);
            }else
            {
                comboBox1.Items.Add(type);
            }
        }


        // 打勾或取消打勾后触发
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 3)
            {
                DataGridViewCell dgcell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string title = (string)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                string type = (string)dataGridView1.Rows[e.RowIndex].Cells[1].Value;
                bool ischk1 = (bool)dgcell.FormattedValue;//选中前
                bool ischk2 = (bool)dgcell.EditedFormattedValue;//选中后
                string sql;
                if(ischk2 == false)
                {
                    sql = string.Format("update events set state = 0 where title = '{0}' and type = '{1}'", title, type);
                }else
                {
                    sql = string.Format("update events set state = 1 where title = '{0}' and type = '{1}'", title, type);
                }
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            loadEvents();
        }


        //private void button7_Click(object sender, EventArgs e)
        //{

        //    Task.Run(() =>
        //    {
        //        SpeechSynthesizer speech = new SpeechSynthesizer();
        //        speech.Rate = 1;
        //        speech.SelectVoice("Microsoft Huihui Desktop");// 设置播音员
        //        speech.Volume = 100;
        //        speech.Speak("net作业");
        //    });
        //}


        //每分钟触发这个函数，然后看有没有到时间的事件，有的话，就发出声音
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 遍历events这个数组，判断是否到时间了
            for (int i = 0; i < events.Count; i++)
            {
                Event ev = (Event)events[i];
                if (DateTime.Now >= ev.getTime())
                {
                    speaker.Speak(ev.getTitle());
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string content;
            for(int i = 0; i<dataGridView1.SelectedRows.Count;i++)
            {
                content = (string)dataGridView1.SelectedRows[i].Cells[0].Value;
                speaker.Speak(content);
                break;
            }
        }
    }
}
