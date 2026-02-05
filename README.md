# Library-Management-System
Library Management System developed using C# Windows Forms and MSSQL.

Library Automation System (Kütüphane Otomasyon Sistemi)

C# ve MSSQL kullanılarak geliştirilmiş, katmanlı mimariye uygun masaüstü kütüphane yönetim projesi.
(A desktop library management project developed using C# and MSSQL.)

Özellikler (Features)
Bu proje ile bir kütüphanenin temel ihtiyaçları dijitalleştirilmiştir:

* Kitap İşlemleri: Kitap Ekleme, Silme, Güncelleme ve Listeleme.
* Üye İşlemleri: Üye kaydı oluşturma ve yönetimi.
* Emanet (Ödünç) Sistemi:
    * Hangi üyenin hangi kitabı aldığının takibi.
    * İade tarihi kontrolü.
    * Stok takibi (Emanetteki kitap başkasına verilemez).
* Görsel Arayüz: Kullanıcı dostu Windows Forms tasarımı.
* İstatistikler: Ana ekranda toplam kitap, üye ve emanet sayıları (Dashboard).

Kullanılan Teknolojiler (Technologies)
* Language: C# (.NET Framework)
* Database: Microsoft SQL Server (MSSQL)
* IDE: Visual Studio 2022
* Architecture: ADO.NET (Veri tabanı bağlantısı için)

Kurulum (Installation)

Projeyi kendi bilgisayarınızda çalıştırmak için:

1.  Repoyu indirin: `git clone https://github.com/KULLANICIADIN/Library-Automation-System.git`
2.  `LibraryDbSetup.sql` dosyasını SQL Server'da çalıştırarak veritabanını oluşturun.
3.  Visual Studio'da `LibraryAutomation.sln` dosyasını açın.
4.  `App.config` veya bağlantı kodundaki `Data Source` kısmını kendi SQL Server adınıza göre güncelleyin.
5.  Projeyi başlatın! 🚀

---
Developed by Yiğit Kaan.
Computer Engineering Student
