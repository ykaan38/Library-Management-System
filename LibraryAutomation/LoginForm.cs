using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LibraryAutomation
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string kullanici_adi = txtUsername.Text;
            string sifre = txtPassword.Text;

            SqlConnection baglanti = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=LibraryDb;Integrated Security=True");

            try
            {
                baglanti.Open(); 

                string sorgu = "SELECT * FROM Admins WHERE Username=@u AND Password=@p";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                // Kutudan gelen verileri güvenli şekilde SQL'e monte et
                komut.Parameters.AddWithValue("@u", kullanici_adi);
                komut.Parameters.AddWithValue("@p", sifre);

                SqlDataReader okuyucu = komut.ExecuteReader();

                if (okuyucu.Read()) 
                {
                    MessageBox.Show("Giriş Başarılı! Hoşgeldiniz.", "Süper", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainForm anaSayfa = new MainForm();
                    anaSayfa.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre yanlış!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                baglanti.Close();

                baglanti.Close(); 
            }
            catch (Exception ex)
            {
                // Bağlantı hatası olursa buraya düşer
                MessageBox.Show("Veritabanına bağlanırken hata oluştu: " + ex.Message);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
