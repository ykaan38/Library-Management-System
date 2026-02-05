using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryAutomation
{
    public partial class MembersForm : Form
    {
        public MembersForm()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=LibraryDb;Integrated Security=True");

        int secilenUyeId = 0;

        void Listele()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Members", baglanti);
            DataTable tablo = new DataTable();
            da.Fill(tablo);
            dataGridView1.DataSource = tablo;

            baglanti.Close();
        }

        private void MembersForm_Load(object sender, EventArgs e)
        {
            Listele();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
        
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Lütfen en az Ad Soyad ve Telefon girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO Members (FullName, Phone, Email) VALUES (@ad, @tel, @mail)", baglanti);

                // DÜZELTME 2: Verileri kutulardan (TextBox) alıyoruz
                komut.Parameters.AddWithValue("@ad", txtName.Text);
                komut.Parameters.AddWithValue("@tel", txtPhone.Text);
                komut.Parameters.AddWithValue("@mail", txtMail.Text);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Üye başarıyla kaydedildi.");
                Listele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (secilenUyeId == 0)
            {
                MessageBox.Show("Lütfen silinecek üyeyi seçin.");
                return;
            }

            if (MessageBox.Show("Bu üyeyi silmek istiyor musun?", "Sil", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("DELETE FROM Members WHERE Id=@id", baglanti);
                    komut.Parameters.AddWithValue("@id", secilenUyeId);
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    Listele();
                    Temizle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (secilenUyeId == 0)
            {
                MessageBox.Show("Lütfen güncellenecek üyeyi seçin.");
                return;
            }

            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("UPDATE Members SET FullName=@ad, Phone=@tel, Email=@mail WHERE Id=@id", baglanti);

                // DÜZELTME 3: Güncelleme yaparken de kutuları kullanıyoruz
                komut.Parameters.AddWithValue("@ad", txtName.Text);
                komut.Parameters.AddWithValue("@tel", txtPhone.Text);
                komut.Parameters.AddWithValue("@mail", txtMail.Text);
                komut.Parameters.AddWithValue("@id", secilenUyeId);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Üye bilgileri güncellendi.");
                Listele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                secilenUyeId = int.Parse(row.Cells[0].Value.ToString());

                // DÜZELTME 4: Veritabanından gelen bilgiyi Etikete değil, Kutuya yazıyoruz
                txtName.Text = row.Cells[1].Value.ToString();
                txtPhone.Text = row.Cells[2].Value.ToString();
                txtMail.Text = row.Cells[3].Value.ToString();
            }
        }

        void Temizle()
        {
            // DÜZELTME 5: Temizlerken kutuları temizliyoruz
            txtName.Text = "";
            txtPhone.Text = "";
            txtMail.Text = "";
            secilenUyeId = 0;
        }
    }
}