using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace AplicatieFreeBook
{
    public partial class MeniuFreeBook : Form
    {
        public MeniuFreeBook(string email)
        {
            InitializeComponent();
            label1.Text = "Email utilizator:";
            label1.Text += email;
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\FreeBook.mdf;Integrated Security=True");
            adress = email;
        }

        SqlConnection con;
        SqlCommand cmd, command;
        StreamReader reader;
        SqlDataReader r;
        string adress;
        int[] ids= new int[100];
        int[] id_carti = new int[100];
        string[] date = new string[100];
        string id, anul;
        string[] lunile_anului = {"nimic", "Ian", "Feb","Mar", "Apr","Mai","Iun","Iul","Aug","sep","Oct","Nov","dEC"};
        int[] frecv = new int[20];
        int[] frev_id = new int[100];
        string title;
        int nr = 0;
        int[] idmax= new int[6];
        int[] maxime = new int[6];
        int mx;

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==3)
            {
                try
                {
                 
                    con.Open();
                    DateTime time = DateTime.Today.AddDays(-30);
                    cmd = new SqlCommand(String.Format("SELECT * FROM imprumut WHERE data_imprumut>'{0}' AND email='{1}';", time, adress), con);
                    r = cmd.ExecuteReader();
                    int cnt = 0;
                    while(r.Read())
                    {
                        cnt++;
                    }
                    r.Close();
                    con.Close();

                    con.Open();
                    if (cnt>=3)
                    {
                        MessageBox.Show("PREA MULTE IMPRUMUTURI");
                    }
                    else
                    {
                        cmd = new SqlCommand(String.Format("INSERT INTO imprumut VALUES ({0},'{1}','{2}');",ids[e.RowIndex],adress,DateTime.Today.ToString() ) ,con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        incarcare();
                    }
                    con.Close();
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        public void incarcare()
        {
            try
            {
                dataGridView2.Rows.Clear();
                con.Open();
                cmd = new SqlCommand(String.Format("SELECT * FROM imprumut WHERE email='{0}';", adress), con);
                r = cmd.ExecuteReader();
                int cnt = 0;
                while (r.Read())
                {
                    int val = Convert.ToInt32(r[1]);
                    id_carti[cnt] = val;
                    date[cnt] = r[3].ToString();
                    cnt++;
                }
                r.Close();
                int uwu = 1;
                for (int i = 0; i < cnt; i++)
                {
                    cmd = new SqlCommand(String.Format("SELECT * FROM carti WHERE id_carte={0};", id_carti[i]), con);
                    r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        DateTime dateTime = DateTime.Parse(date[i]);
                        DateTime time = dateTime.AddDays(-30);
                        dataGridView2.Rows.Add(uwu, r[1], r[2], r[3], date[i], time);
                        uwu++;
                    }
                    r.Close();
                }
                con.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            con.Close();

            //colour
            int disponibile = 0;
            foreach (DataGridViewRow Datarow in dataGridView2.Rows)
            {
                if (Datarow.Cells[4].Value != null)
                {
                    string mx = Datarow.Cells[4].Value.ToString(); //data maxima de disponibilitate a cartii
                    DateTime val1 = DateTime.Parse(mx);
                    if (val1 < DateTime.Today)
                    {
                        Datarow.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        Datarow.DefaultCellStyle.BackColor = Color.Green;
                        disponibile++;
                    }
                }
            }

            //progressbar & label
            label2.Text = "Disponibilitate imprumut:";
            label2.Text += disponibile.ToString();
            progressBar1.Value = disponibile;
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if(dataGridView2.Rows[index].DefaultCellStyle.BackColor==Color.Red)
            {
                MessageBox.Show("Perioada Imprumutului expirata");
            }
            else
            {
                DataGridViewRow row = dataGridView2.Rows[index];
                string file = row.Cells[1].Value.ToString();
                //cauti indexul corecpunzator acestei carti ca sa-l transmiti ca paramteru
                con.Open();
                cmd = new SqlCommand(String.Format("SELECT * FROM carti WHERE titlu='{0}';",file), con);
                r = cmd.ExecuteReader();
                while(r.Read())
                {
                    id = r[0].ToString();
                }
                con.Close();
                r.Close();
                MessageBox.Show(id);
                AfiseazaCarte callable = new AfiseazaCarte(id);
                callable.Show();
                this.Hide();
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
            //nimic
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //chartul va prezenta evidența numărul de utilizatori în fiecare lună, pentru toți utilizatorii care au făcut împrumuturi, pe baza anului ales
            try
            {
                anul = comboBox1.SelectedItem.ToString();
                chart();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void chart()
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT * FROM imprumut;",con);
                r = cmd.ExecuteReader();
                while(r.Read())
                {
                    string data = r[3].ToString();
                    string[] bucati = data.Split('/');
                    string[] bucatele = bucati[2].Split(' ');
                    string an = bucatele[0];
                    if(String.Compare(anul,an)==0)
                    {
                        //incrementezi la luna corespunzatoare in vectorul de frecventa
                        int luna = Convert.ToInt32(bucati[0]);
                        frecv[luna]++;
                    }
                }
                r.Close();
                con.Close();

                //contruiesti tabelul propriu zis
                for(int cnt=1;cnt<=12; cnt++)
                {
                    chart1.Series["Luna"].Points.AddXY(lunile_anului[cnt], frecv[cnt]);
                    chart1.Update();
                }
            }
            catch(Exception eX)
            {
                MessageBox.Show(eX.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void pie()
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM imprumut",con);
            r = cmd.ExecuteReader();
            while(r.Read())
            {
                frev_id[Convert.ToInt32(r[1])]++;
            }
            r.Close();
            //cauti maximul de 4 ori ca sa stii ce id-uri cauti
            for(int cnt=0; cnt<100; cnt++)
            {
                if (frev_id[cnt] >mx)
                {
                    idmax[1] = cnt;
                }
            }
            mx = 0;
            maxime[1] = frev_id[idmax[1]];
            frev_id[idmax[1]] = 0;
            for (int cnt = 0; cnt < 100; cnt++)
            {
                if (frev_id[cnt] > mx)
                {
                    idmax[2] = cnt;
                }
            }
            mx = 0;
            maxime[2] = frev_id[idmax[2]];
            frev_id[idmax[2]] = 0;
            for (int cnt = 0; cnt < 100; cnt++)
            {
                if (frev_id[cnt] > mx)
                {
                    idmax[3] = cnt;
                }
            }
            maxime[3] = frev_id[idmax[3]];
            frev_id[idmax[3]] = 0;
            mx = 0;
            for (int cnt = 0; cnt < 100; cnt++)
            {
                if (frev_id[cnt] > mx)
                {
                    idmax[4] = cnt;
                }
            }
            maxime[4] = frev_id[idmax[4]];
            for (int cnt = 1; cnt < 5; cnt++)
            {
                    cmd = new SqlCommand(String.Format("SELECT * FROM carti WHERE id_carte='{0}';", idmax[cnt].ToString()), con);
                    r = cmd.ExecuteReader();
                    while(r.Read())
                    {
                        title = r[1].ToString();
                    }
                    r.Close();
                    //add to pie
                    chart2.Series["CartiImprumutate"].Points.AddXY(title, maxime[cnt]);
                    nr++;
            }
            con.Close();


        }

        private void MeniuFreeBook_Load(object sender, EventArgs e)
        {
            try
            {
                incarcare();
                con.Open();
                cmd = new SqlCommand(String.Format("SELECT * FROM imprumut WHERE data_imprumut<'{0}' AND email!='{1}';", DateTime.Today.ToString(), adress), con);
                r = cmd.ExecuteReader();
                int cnt = 0;
                while (r.Read())
                {
                    int val = Convert.ToInt32(r[1]);
                    ids[cnt] = val;
                    cnt++;
                }
                r.Close();
                for(int i=0; i<cnt; i++)
                {
                    cmd = new SqlCommand(String.Format("SELECT * FROM carti WHERE id_carte={0};", ids[i]), con);
                    r = cmd.ExecuteReader();
                    while(r.Read())
                        dataGridView1.Rows.Add(r[1], r[2], r[3]);
                    r.Close();
                }
                con.Close();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

            //facem chartul tip pie
            pie();

        }
    }
}
