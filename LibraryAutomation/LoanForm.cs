using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibraryAutomation
{
    public partial class LoanForm : Form
    {
        public LoanForm()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=LibraryDb;Integrated Security=True");

        // --- 1. AÇILIR LİSTELERİ DOLDURMA (Sihirli Kısım) ---
        void VerileriYukle()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            // A) Üyeleri Getir ve Listeye Doldur
            SqlDataAdapter daUye = new SqlDataAdapter("SELECT Id, FullName FROM Members", baglanti);
            DataTable dtUye = new DataTable();
            daUye.Fill(dtUye);

            cbMember.DisplayMember = "FullName"; // Ekranda Ad Soyad görünsün
            cbMember.ValueMember = "Id";         // Arka planda ID tutulsun
            cbMember.DataSource = dtUye;

            // B) Sadece RAFTA OLAN (Müsait) Kitapları Getir (Status=1)
            SqlDataAdapter daKitap = new SqlDataAdapter("SELECT Id, Title FROM Books WHERE Status=1", baglanti);
            DataTable dtKitap = new DataTable();
            daKitap.Fill(dtKitap);

            cbBook.DisplayMember = "Title"; // Ekranda Kitap Adı görünsün
            cbBook.ValueMember = "Id";      // Arka planda ID tutulsun
            cbBook.DataSource = dtKitap;

            baglanti.Close();
        }

        // --- 2. EMANET LİSTESİNİ GÖSTERME (JOIN İşlemi) ---
        void EmanetListele()
        {
            if (baglanti.State == ConnectionState.Closed) baglanti.Open();

            // Burada 3 tabloyu birleştiriyoruz (JOIN) ki ID yerine İsimler görünsün
            string sorgu = @"
                SELECT 
                    Loans.Id, 
                    Members.FullName AS 'Üye', 
                    Books.Title AS 'Kitap', 
                    Loans.LoanDate AS 'Veriliş Tarihi', 
                    Loans.DueDate AS 'Teslim Tarihi',
                    Loans.IsReturned AS 'İade Durumu'
                FROM Loans
                INNER JOIN Members ON Loans.MemberId = Members.Id
                INNER JOIN Books ON Loans.BookId = Books.Id";

            SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            baglanti.Close();
        }

        // --- FORM AÇILINCA ---
        private void LoanForm_Load(object sender, EventArgs e)
        {
            VerileriYukle();
            EmanetListele();
        }

        // --- EMANET VER BUTONU ---
        private void btnLend_Click(object sender, EventArgs e)
        {
            try
            {
                if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                // 1. Emanet Tablosuna Ekle
                string sorgu = "INSERT INTO Loans (MemberId, BookId, LoanDate, DueDate, IsReturned) VALUES (@uye, @kitap, @tarih1, @tarih2, 0)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.Parameters.AddWithValue("@uye", cbMember.SelectedValue); // Seçilen üyenin ID'si
                komut.Parameters.AddWithValue("@kitap", cbBook.SelectedValue); // Seçilen kitabın ID'si
                komut.Parameters.AddWithValue("@tarih1", DateTime.Now);        // Bugün
                komut.Parameters.AddWithValue("@tarih2", dtpDate.Value);       // Seçilen tarih

                komut.ExecuteNonQuery();

                // 2. Kitabın Durumunu "MEŞGUL" (0) Yap
                SqlCommand komutDurum = new SqlCommand("UPDATE Books SET Status = 0 WHERE Id = @kId", baglanti);
                komutDurum.Parameters.AddWithValue("@kId", cbBook.SelectedValue);
                komutDurum.ExecuteNonQuery();

                baglanti.Close();

                MessageBox.Show("Kitap emanet verildi!");
                VerileriYukle(); // Listeleri yenile (Verilen kitap listeden düşsün)
                EmanetListele(); // Tabloyu yenile
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
                if (baglanti.State == ConnectionState.Open) baglanti.Close();
            }
        }

        // --- İADE AL BUTONU ---
        private void btnReturn_Click(object sender, EventArgs e)
        {
            // Tablodan seçim yapılmış mı?
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int secilenIslemId = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                bool iadeEdildiMi = bool.Parse(dataGridView1.CurrentRow.Cells[5].Value.ToString());

                if (iadeEdildiMi)
                {
                    MessageBox.Show("Bu kitap zaten iade alınmış!");
                    return;
                }

                try
                {
                    if (baglanti.State == ConnectionState.Closed) baglanti.Open();

                    // 1. Emaneti "İade Edildi" (1) olarak işaretle
                    SqlCommand komut = new SqlCommand("UPDATE Loans SET IsReturned = 1 WHERE Id = @id", baglanti);
                    komut.Parameters.AddWithValue("@id", secilenIslemId);
                    komut.ExecuteNonQuery();

                    // 2. Kitabı bulmak için biraz dedektiflik yapalım (Kitap Adından ID bulma)
                    // (Basitlik için: Kitabın ID'sini loans tablosundan çekmek daha doğru ama burada kısa yoldan grid yenileyeceğiz)
                    // Hızlı çözüm: Kitabı tekrar "MÜSAİT" (1) yapmamız lazım.
                    // Bunun için gridde gizli Kitap ID tutmadığımızdan, şu anlık sadece Loan tablosunu güncelleyelim.
                    // *** PROFESYONEL İPUCU: Gerçek projede Kitap ID'yi de çekmemiz gerekir. ***
                    // Şimdilik emaneti kapatalım, kitabı manuel düzeltiriz veya sorguyu geliştiririz.

                    // Kitap ID'sini bulup Status=1 yapalım:
                    string kitapBulSorgu = "SELECT BookId FROM Loans WHERE Id = @lid";
                    SqlCommand kmtBul = new SqlCommand(kitapBulSorgu, baglanti);
                    kmtBul.Parameters.AddWithValue("@lid", secilenIslemId);
                    int kitapId = (int)kmtBul.ExecuteScalar(); // Kitap ID'sini bulduk

                    SqlCommand komutKitap = new SqlCommand("UPDATE Books SET Status = 1 WHERE Id = @kid", baglanti);
                    komutKitap.Parameters.AddWithValue("@kid", kitapId);
                    komutKitap.ExecuteNonQuery();

                    baglanti.Close();

                    MessageBox.Show("Kitap iade alındı, rafa geri kondu.");
                    VerileriYukle();
                    EmanetListele();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen listeden iade edilecek işlemi seçin.");
            }
        }
    }
}