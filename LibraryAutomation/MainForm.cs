using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryAutomation
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void kitapİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            BooksForm kitapFormu = new BooksForm();
            kitapFormu.MdiParent = this;   // Bu form, Ana Formun (MainForm) içinde açılsın
            kitapFormu.Show();
        }

        private void üyeİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MembersForm uyeFormu = new MembersForm();
            uyeFormu.MdiParent = this; 
            uyeFormu.Show();
        }

        private void emanetİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoanForm emanetFormu = new LoanForm();
            emanetFormu.MdiParent = this;
            emanetFormu.Show();
        }
    }
}
