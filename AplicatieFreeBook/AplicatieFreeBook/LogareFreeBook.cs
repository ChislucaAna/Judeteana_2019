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
    public partial class LogareFreeBook : Form
    {
        public LogareFreeBook()
        {
            InitializeComponent();
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\FreeBook.mdf;Integrated Security=True");
        }

        string email, parola;
        SqlConnection con;
        SqlCommand cmd, command;
        StreamReader reader;
        SqlDataReader r;

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            parola = textBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCommand(String.Format("SELECT * FROM utilizatori WHERE email='{0}' AND parola='{1}';", email, parola), con);
                r = cmd.ExecuteReader();
                if(r.Read()==false)
                {
                    MessageBox.Show("Eroare autentificare!,");
                }
                else
                {
                    MeniuFreeBook callable = new MeniuFreeBook(email);
                    callable.Show();
                    this.Hide();
                }
                con.Close();
                r.Close();
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            email = textBox1.Text;
        }
    }
}
