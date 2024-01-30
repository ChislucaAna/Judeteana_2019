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

namespace AplicatieFreeBook
{
    public partial class AfiseazaCarte : Form
    {
        public AfiseazaCarte(string fisier)
        {
            InitializeComponent();
            fisier += ".pdf";
            string p = Path.GetFullPath(fisier); //doesnt support format??
            webBrowser1.Url = new Uri(p);
        }
    }
}
