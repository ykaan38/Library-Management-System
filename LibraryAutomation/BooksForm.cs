using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryAutomation
{
    public partial class BooksForm : Form
    {
        public BooksForm()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=LibraryDb;Integrated Security=True");

        // Seçili olan kitabın ID'sini tutmak için (Silme ve Güncelleme için lazım)
        int secilenKitapId = 0;

        // --- LİSTELEME METODU ---
        void Listele()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Books", baglanti);
            DataTable tablo = new DataTable();
            da.Fill(tablo);
            dataGridView1.DataSource = tablo; // Veriyi ızgaraya dök

            baglanti.Close();
        }

        // --- FORM YÜKLENİNCE ---
        private void BooksForm_Load(object sender, EventArgs e)
        {
            Listele();
        }

        // --- EKLE BUTONU ---
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. BOŞ VERİ KONTROLÜ (Validation)
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtAuthor.Text) || string.IsNullOrWhiteSpace(txtPage.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Kod buradan aşağıya inmez, işlemi durdurur.
            }

            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO Books (Title, Author, PageCount) VALUES (@t, @a, @p)", baglanti);
                
                komut.Parameters.AddWithValue("@t", txtTitle.Text);
                komut.Parameters.AddWithValue("@a", txtAuthor.Text);
                komut.Parameters.AddWithValue("@p", int.Parse(txtPage.Text)); // Sayı hatası vermesin diye çevirdik

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Kitap eklendi.");
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
            if (secilenKitapId == 0)
            {
                MessageBox.Show("Lütfen silinecek kitabı listeden seçin.");
                return;
            }

            DialogResult cevap = MessageBox.Show("Bu kitabı silmek istediğine emin misin?", "Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cevap == DialogResult.Yes)
            {
                try
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("DELETE FROM Books WHERE Id=@id", baglanti);
                    komut.Parameters.AddWithValue("@id", secilenKitapId);
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    Listele();
                    Temizle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silinirken hata oluştu: " + ex.Message);
                }
            }
        }
 
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (secilenKitapId == 0)
            {
                MessageBox.Show("Lütfen güncellenecek kitabı listeden seçin.");
                return;
            }

            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("UPDATE Books SET Title=@t, Author=@a, PageCount=@p WHERE Id=@id", baglanti);

                komut.Parameters.AddWithValue("@t", txtTitle.Text);
                komut.Parameters.AddWithValue("@a", txtAuthor.Text);
                komut.Parameters.AddWithValue("@p", txtPage.Text);
                komut.Parameters.AddWithValue("@id", secilenKitapId);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Kitap bilgileri güncellendi.");
                Listele();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Başlığa (Header) tıklayınca hata vermesin
            if (e.RowIndex >= 0) 
            {
                // Tıklanan satırı al
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Kutuları doldur
                secilenKitapId = int.Parse(row.Cells[0].Value.ToString()); // ID'yi hafızaya al (Gizli kahraman)
                txtTitle.Text = row.Cells[1].Value.ToString();
                txtAuthor.Text = row.Cells[2].Value.ToString();
                txtPage.Text = row.Cells[3].Value.ToString();
            }
        }
  
        void Temizle()
        {
            txtTitle.Text = "";
            txtAuthor.Text = "";
            txtPage.Text = "";
            secilenKitapId = 0; 
        }
    }
}