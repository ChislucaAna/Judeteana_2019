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
    public partial class CreeazaContFreeBook : Form
    {
        public CreeazaContFreeBook()
        {
            InitializeComponent();
            con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\FreeBook.mdf;Integrated Security=True");
        }

        string email, nume, prenume, parola1, parola2;
        SqlConnection con;
        SqlCommand cmd, command;
        StreamReader reader;
        SqlDataReader r;

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            parola1 = textBox5.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            parola2 = textBox4.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            prenume = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            nume = textBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (email == null || nume == null || prenume == null || parola1 == null || parola2==null)
            {
                MessageBox.Show("completati toate campurile");
            }
            else
            {
                try
                {
                    con.Open();
                    cmd = new SqlCommand(String.Format("SELECT * FROM utilizatori WHERE email='{0}'", email), con);
                    r = cmd.ExecuteReader();
                    if (r.Read()==true)
                    {
                        MessageBox.Show("emailul este utilizat deja");
                        r.Close();
                    }
                    else
                    {
                        r.Close();
                        if (String.Compare(parola2,parola1)!=0)
                        {
                            MessageBox.Show("PAROLELE SUNT DIFERITE");
                        }
                        else
                        {
                            cmd = new SqlCommand(String.Format("INSERT INTO utilizatori VALUES('{0}','{1}','{2}','{3}');", email, parola1, nume, prenume), con);
                            cmd.ExecuteNonQuery();
                            MeniuFreeBook callable = new MeniuFreeBook(email);
                            callable.Show();
                            this.Hide();
                        }
                    }
                    con.Close();
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            email = textBox1.Text;
        }
    }
}
