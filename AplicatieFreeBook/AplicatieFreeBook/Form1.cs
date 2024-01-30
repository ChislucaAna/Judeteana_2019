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
    public partial class FreeBookHome : Form
    {
        SqlConnection con;
        SqlCommand cmd,command;
        StreamReader reader;
        SqlDataReader r;
        string id;
        public FreeBookHome()
        {
            InitializeComponent();
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\FreeBook.mdf;Integrated Security=True");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreeazaContFreeBook callable = new CreeazaContFreeBook();
            callable.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LogareFreeBook callable = new LogareFreeBook();
            callable.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string line;
                reader = new StreamReader("carti.txt");
                while((line=reader.ReadLine())!=null)
                {
                    string[] bucati = line.Split('*');
                    cmd = new SqlCommand(String.Format("INSERT INTO carti VALUES('{0}','{1}','{2}');", bucati[0], bucati[1], bucati[2]), con);
                    cmd.ExecuteNonQuery();
                }
                reader.Close();
                reader = new StreamReader("utilizatori.txt");
                while ((line = reader.ReadLine()) != null)
                {
                    string[] bucati = line.Split('*');
                    cmd = new SqlCommand(String.Format("INSERT INTO utilizatori VALUES('{0}','{1}','{2}','{3}');", bucati[0], bucati[1], bucati[2], bucati[3]), con);
                    cmd.ExecuteNonQuery();
                }
                reader.Close();
                reader = new StreamReader("imprumuturi.txt");
                while ((line = reader.ReadLine()) != null)
                {
                    string[] bucati = line.Split('*');
                    string nume = bucati[0];
                    command = new SqlCommand(String.Format("SELECT * FROM carti WHERE titlu='{0}';", nume), con);
                    r = command.ExecuteReader();
                    while(r.Read())
                    {
                        id = r[0].ToString();
                    }
                    r.Close();
                    cmd = new SqlCommand(String.Format("INSERT INTO imprumut VALUES({0},'{1}','{2}');", id, bucati[1], bucati[2]), con);
                    cmd.ExecuteNonQuery();
                }
                reader.Close();
                con.Close();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
