using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public partial class Form1 : Form
    {
       
        private Kule seciliKule = null;
        private bool menzilGosteriliyor = false; 

        // Düşmanların izleyeceği yolu tutan koordinat listesi
        private List<Point> yolKoordinatlari = new List<Point>();

        private List<Kule> aktifKuleler = new List<Kule>(); // Haritadaki aktif kuleleri tutar

        private Type secilenKuleTipi = null; 
        private int KuleFiyati; 

        public Form1()
        {
            InitializeComponent();
            this.oyunDongusuTimer.Tick += OyunDongusuTimer_Tick;
            this.pnlOyunAlani.MouseClick += pnlOyunAlani_MouseClick;
            yolKoordinatlari.Add(new Point(50, 300));
            yolKoordinatlari.Add(new Point(200, 300));
            yolKoordinatlari.Add(new Point(200, 150));
            yolKoordinatlari.Add(new Point(400, 150));
            yolKoordinatlari.Add(new Point(400, 450));
            yolKoordinatlari.Add(new Point(600, 450));
            yolKoordinatlari.Add(new Point(600, 300));
            yolKoordinatlari.Add(new Point(750, 300)); 
            pnlOyunAlani.Paint += PnlOyunAlani_Paint;
        }

        private void PnlOyunAlani_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen yolKalemi = new Pen(Color.White, 40);
            Point[] yolNoktalari = yolKoordinatlari.ToArray();
            g.DrawLines(yolKalemi, yolNoktalari);
            yolKalemi.Color = Color.Green; 
            g.DrawEllipse(yolKalemi, yolNoktalari[0].X - 15, yolNoktalari[0].Y - 15, 30, 30);
            yolKalemi.Color = Color.Maroon; 
            g.DrawEllipse(yolKalemi, yolNoktalari[yolNoktalari.Length - 1].X - 15, yolNoktalari[yolNoktalari.Length - 1].Y - 15, 30, 30);
            yolKalemi.Dispose(); 

            // --- Menzil Görselleştirme ---
            if (menzilGosteriliyor && seciliKule != null)
            {
                int yaricap = seciliKule.Menzil;
                int cap = yaricap * 2;

                
                int x = seciliKule.Konum.X - yaricap + (seciliKule.GorselNesne.Width / 2);
                int y = seciliKule.Konum.Y - yaricap + (seciliKule.GorselNesne.Height / 2);

                
                using (Pen pen = new Pen(Color.FromArgb(100, 255, 255, 255), 2)) 
                {
                   
                    e.Graphics.DrawEllipse(pen, x, y, cap, cap);
                }

                
                using (Brush brush = new SolidBrush(Color.FromArgb(20, 0, 150, 255))) 
                {
                    e.Graphics.FillEllipse(brush, x, y, cap, cap);
                }
            }
        }


        private List<Dusman> aktifDusmanlar = new List<Dusman>();
        private int mevcutDalga = 1;
        private int toplamDalga = 5; 
        private int dalgaIciDusmanSayisi;
        private int spawnEdilenDusmanSayisi = 0;
        private int spawnAraligi = 20; 
        private int spawnSayaci = 0;


        private int altin = 300;    
        private int can = 100;  
        private int skor = 0;
        private int _mevcutDalga = 1;
        private const int ToplamDalga = 5;



        private void DalgaBaslat(int dalgaNumarasi)
        {
            // Dalga Zorluk Ayarı:
            dalgaIciDusmanSayisi = 10 + (dalgaNumarasi * 5); 
            float hiz = 1.2f + (dalgaNumarasi * 1.0f); 

            MessageBox.Show($"Dalga {dalgaNumarasi} Başladı! {dalgaIciDusmanSayisi} Düşman Geliyor.");

            spawnEdilenDusmanSayisi = 0;
            spawnSayaci = 0;
            oyunDongusuTimer.Start(); 
            DegerleriGuncelle(); 
        }

        private void DusmanlariTemizle()
        {
            for (int i = aktifDusmanlar.Count - 1; i >= 0; i--)
            {
                Dusman dusman = aktifDusmanlar[i];

                if (dusman.Can <= 0)
                {
                    altin += dusman.AltinDegeri;
                    skor += 1;
                    pnlOyunAlani.Controls.Remove(dusman.GetGorselNesne());
                    aktifDusmanlar.RemoveAt(i);
                }
            }
        }

        private bool YolUzerindeMi(Point p)
        {
            int yolKalinligi = 40; 

            for (int i = 0; i < yolKoordinatlari.Count - 1; i++)
            {
                Point p1 = yolKoordinatlari[i];
                Point p2 = yolKoordinatlari[i + 1];
                if (p1.X == p2.X)
                {
                    if (p.X >= p1.X - yolKalinligi / 2 && p.X <= p1.X + yolKalinligi / 2 &&
                        p.Y >= Math.Min(p1.Y, p2.Y) && p.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        return true;
                    }
                }
                // Yatay segment kontrolü
                else if (p1.Y == p2.Y)
                {
                    if (p.Y >= p1.Y - yolKalinligi / 2 && p.Y <= p1.Y + yolKalinligi / 2 &&
                        p.X >= Math.Min(p1.X, p2.X) && p.X <= Math.Max(p1.X, p2.X))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OyunDongusuTimer_Tick(object sender, EventArgs e)
        {
            // A) Düşman Spawn Etme Mantığı
            if (spawnEdilenDusmanSayisi < dalgaIciDusmanSayisi)
            {
                spawnSayaci++;
                if (spawnSayaci >= spawnAraligi)
                {
                    Point baslangicKonumu = yolKoordinatlari[0];
                    // Yeni düşmanı oluşturma işlemi
                    Dusman yeniDusman = new Dusman(
                        baslangicKonumu,
                        can: 120 + (mevcutDalga * 100),
                        hiz: 2.0f, 
                        altin: 5,
                        renk: Color.Red
                    );
                    

                    // Düşmanı haritaya ekleme işkemi
                    pnlOyunAlani.Controls.Add(yeniDusman.GetGorselNesne());
                    yeniDusman.GetGorselNesne().BringToFront();
                    aktifDusmanlar.Add(yeniDusman);

                    spawnEdilenDusmanSayisi++;
                    spawnSayaci = 0;
                }
            }

            // B) Aktif Düşmanları Hareket Ettirme 
             foreach (Dusman dusman in aktifDusmanlar)
             {
                 dusman.HareketEt(yolKoordinatlari);
             }

            // C) Dalga Bitiş Kontrolü
            if (spawnEdilenDusmanSayisi >= dalgaIciDusmanSayisi && aktifDusmanlar.Count == 0)
            {
                oyunDongusuTimer.Stop(); 
                mevcutDalga++;
                if (mevcutDalga <= toplamDalga)
                {
                    DalgaBaslat(mevcutDalga);
                }
                else
                {
                    MessageBox.Show("Tebrikler! Oyunu Kazandınız."); 
                    OyunuTemizle();
                    Close();
                }
            }


            
            foreach (Dusman dusman in aktifDusmanlar.ToList()) 
            {
                dusman.HareketEt(yolKoordinatlari);

                
                if (dusman.GidilecekHedefIndex >= yolKoordinatlari.Count)
                {
                    
                    can= can -7;
                    DegerleriGuncelle();
                    aktifDusmanlar.Remove(dusman);
                    pnlOyunAlani.Controls.Remove(dusman.GetGorselNesne());
                    if (can <= 0)
                    {
                       oyunDongusuTimer.Stop();
                       OyunuTemizle();
                       MessageBox.Show($"Oyun Bitti! Skorunuz: {skor}"); 
                       Close();
                    }
                }
            }

            // kule saldırı mekaniği
            foreach (Kule kule in aktifKuleler)
            {
                kule.SaldiriSayaci++;
                if (kule.SaldiriSayaci >= kule.GerekliTickSayisi)
                {
                    // Polymorphism: Her kule kendi Saldir() metodunu çağırır.
                    kule.Saldir(aktifDusmanlar);
                    DusmanlariTemizle();

                    kule.SaldiriSayaci = 0; 
                }
            }

            
            // ---D) Dalga Bitiş Kontrolü---

            if (spawnEdilenDusmanSayisi >= dalgaIciDusmanSayisi && aktifDusmanlar.Count == 0)
            {
                oyunDongusuTimer.Stop(); 
                altin += 100; 

                mevcutDalga++; 

                // UI'ı anında güncelle 
                DegerleriGuncelle();

                if (mevcutDalga <= ToplamDalga)
                {
                    MessageBox.Show($"Dalga {mevcutDalga} başlıyor! Ekstra 100 Altın Kazandınız.", "Yeni Dalga");
                    DalgaBaslat(mevcutDalga); 
                }
                else
                {
                    MessageBox.Show("Oyun Kazanıldı");
                } 
            }


            DegerleriGuncelle();
        }

        private void OyunuTemizle()
        {
            // Düşmanları kaldır
            for (int i = aktifDusmanlar.Count - 1; i >= 0; i--)
            {
                Dusman dusman = aktifDusmanlar[i];
                pnlOyunAlani.Controls.Remove(dusman.GetGorselNesne());
            }
            aktifDusmanlar.Clear(); 

            // Kuleleri kaldır 
            for (int i = aktifKuleler.Count - 1; i >= 0; i--)
            {
                Kule kule = aktifKuleler[i];
                pnlOyunAlani.Controls.Remove(kule.GorselNesne);
            }
            aktifKuleler.Clear(); 
        }


        public void DegerleriGuncelle()
        {
            
            lblAltinDeger.Text = $"💰 Altın: {altin}";
            lblCanDeger.Text = $"❤️ Can: {can}";
            lblDalgaDeger.Text = " 🌊 Dalga: " + mevcutDalga.ToString() + "/5";
            lblSkorDeger.Text = $"⭐ Skor: {skor}";
        }

        private void OyunEkrani_Load(object sender, EventArgs e)
        {
            DalgaBaslat(mevcutDalga);
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DalgaBaslat(mevcutDalga);
            
            oyunDongusuTimer.Start();
            button1.Enabled = false;
        }

        private void btnOkKulesi_Click(object sender, EventArgs e)
        {
            KuleFiyati = 150; 
            if (altin < KuleFiyati)
            {
                MessageBox.Show("Yeterli altınınız yok! (100 Altın Gerekli)");
                secilenKuleTipi = null;
                return;
            }
            secilenKuleTipi = typeof(OkKulesi);
        }

        private void btnTopKulesi_Click(object sender, EventArgs e)
        {
            KuleFiyati = 220; 
            if (altin < KuleFiyati)
            {
                MessageBox.Show("Yeterli altınınız yok! (250 Altın Gerekli)");
                secilenKuleTipi = null;
                return;
            }
            secilenKuleTipi = typeof(TopKulesi);
        }

        private void btnBuyuKulesi_Click(object sender, EventArgs e)
        {
            KuleFiyati = 250; 
            if (altin < KuleFiyati)
            {
                MessageBox.Show("Yeterli altınınız yok! (200 Altın Gerekli)");
                secilenKuleTipi = null;
                return;
            }
            secilenKuleTipi = typeof(BuyuKulesi);
        }

        private void btnLazerKulesi_Click(object sender, EventArgs e)
        {
            KuleFiyati = 400; 
            if (altin < KuleFiyati)
            {
                MessageBox.Show("Yeterli altınınız yok! (400 Altın Gerekli)");
                secilenKuleTipi = null;
                return;
            }
            secilenKuleTipi = typeof(LazerKulesi);

        }


        private void pnlOyunAlani_MouseClick(object sender, MouseEventArgs e)
        {
            
            if (secilenKuleTipi == null || e.Button != MouseButtons.Left) return;

            Point tiklananKonum = e.Location;
            Kule yeniKule = null;

            // NDP Gereksinimi: Kuleler yol üzerine yerleştirilemez.
            if (YolUzerindeMi(tiklananKonum)) 
            {
                MessageBox.Show("Kuleler yol üzerine yerleştirilemez!");
                return;
            }

            // Kuleyi dinamik olarak oluşturma (Factory Pattern benzeri dinamik nesne üretimi)
            if (secilenKuleTipi == typeof(OkKulesi))
            {
                yeniKule = new OkKulesi(tiklananKonum);
            }
            else if (secilenKuleTipi == typeof(TopKulesi))
            {
                yeniKule = new TopKulesi(tiklananKonum);
            }
            else if (secilenKuleTipi == typeof(BuyuKulesi))
            {
                yeniKule = new BuyuKulesi(tiklananKonum);
            }
            else if (secilenKuleTipi == typeof(LazerKulesi))
            {
                yeniKule = new LazerKulesi(tiklananKonum);
            }

            if (yeniKule != null)
            {
                // 1. Kuleyi Panele Ekle
                pnlOyunAlani.Controls.Add(yeniKule.GorselNesne);
                yeniKule.GorselNesne.BringToFront(); 
                pnlOyunAlani.Controls.Add(yeniKule.SeviyeEtiketi);
                yeniKule.SeviyeEtiketi.BringToFront();
                // 2. Yönetim Listesine Ekle
                aktifKuleler.Add(yeniKule);

                // 3. Altını Düşür
                altin -= KuleFiyati;
                DegerleriGuncelle(); // UI'ı güncelle

                // Kuleyi seçili yap ve menzilini hemen gösterelim
                seciliKule = yeniKule;
                menzilGosteriliyor = true;
             
                pnlOyunAlani.Invalidate(); // Hemen çizimi tetikl

                // 4. Seçimi Sıfırla
                secilenKuleTipi = null;
                return;
            }

            Kule tiklananKule = null;
            int kuleBoyutu = 40;

            
            foreach (Kule kule in aktifKuleler)
            {
                Rectangle kuleSinirlari = new Rectangle(kule.Konum.X, kule.Konum.Y, kuleBoyutu, kuleBoyutu);

                if (kuleSinirlari.Contains(tiklananKonum))
                {
                    tiklananKule = kule;
                    break;
                }
            }

            // TIKLAMA İLE MENZİL AÇMA/KAPAMA (OLMADI)
            if (tiklananKule != null)
            {
                if (seciliKule == tiklananKule && menzilGosteriliyor)
                {
                    menzilGosteriliyor = false;
                    seciliKule = null;
                }
                else 
                {
                    seciliKule = tiklananKule;
                    menzilGosteriliyor = true;
                }

                pnlOyunAlani.Invalidate(); 
            }
            else
            {
                seciliKule = null;
                menzilGosteriliyor = false;
                pnlOyunAlani.Invalidate();
            }
        }

        private void OkKulelerıYukselt_Click(object sender, EventArgs e)
        {
            int toplamMaliyet = 0;
            foreach (Kule k in aktifKuleler)
            {
                if (k is OkKulesi && k.Seviye < 3) toplamMaliyet += k.YukseltmeMaliyeti;
            }

            if (toplamMaliyet > 0 && altin >= toplamMaliyet)
            {
                altin -= toplamMaliyet; 

                foreach (Kule k in aktifKuleler)
                {
                    if (k is OkKulesi && k.Seviye < 3)
                    {
                        k.Yukselt(); 
                    }
                }
               
                DegerleriGuncelle(); 
                pnlOyunAlani.Invalidate();
            }
            else
            {
                MessageBox.Show(toplamMaliyet == 0 ? "Yükseltilecek kule yok veya hepsi Max Seviye!" : $"Yetersiz Altın! Gerekli: {toplamMaliyet}");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void TopKuleleriYukselt_Click(object sender, EventArgs e)
        {
            int toplamMaliyet = 0;
            foreach (Kule k in aktifKuleler)
            {
                if (k is TopKulesi && k.Seviye < 3) toplamMaliyet += k.YukseltmeMaliyeti;
            }

            if (toplamMaliyet > 0 && altin >= toplamMaliyet)
            {
                altin -= toplamMaliyet; 

                foreach (Kule k in aktifKuleler)
                {
                    if (k is TopKulesi && k.Seviye < 3)
                    {
                        k.Yukselt(); 
                    }
                }


                DegerleriGuncelle(); 
                pnlOyunAlani.Invalidate(); 
            }
            else
            {
                MessageBox.Show(toplamMaliyet == 0 ? "Yükseltilecek kule yok veya hepsi Max Seviye!" : $"Yetersiz Altın! Gerekli: {toplamMaliyet}");
            }
        }

        private void BuyucuKuleYukselt_Click(object sender, EventArgs e)
        {
            int toplamMaliyet = 0;
            foreach (Kule k in aktifKuleler)
            {
                if (k is BuyuKulesi && k.Seviye < 3) toplamMaliyet += k.YukseltmeMaliyeti;
            }

            if (toplamMaliyet > 0 && altin >= toplamMaliyet)
            {
                altin -= toplamMaliyet; 

                foreach (Kule k in aktifKuleler)
                {
                    if (k is BuyuKulesi && k.Seviye < 3)
                    {
                        k.Yukselt(); 
                    }
                }

                DegerleriGuncelle(); 
                pnlOyunAlani.Invalidate(); 
            }
            else
            {
                MessageBox.Show(toplamMaliyet == 0 ? "Yükseltilecek kule yok veya hepsi Max Seviye!" : $"Yetersiz Altın! Gerekli: {toplamMaliyet}");
            }
        }

        private void LazerKuleleriYukselt_Click(object sender, EventArgs e)
        {
            int toplamMaliyet = 0;
            foreach (Kule k in aktifKuleler)
            {
                if (k is LazerKulesi && k.Seviye < 3) toplamMaliyet += k.YukseltmeMaliyeti;
            }

            if (toplamMaliyet > 0 && altin >= toplamMaliyet)
            {
                altin -= toplamMaliyet; 

                foreach (Kule k in aktifKuleler)
                {
                    if (k is LazerKulesi && k.Seviye < 3)
                    {
                        k.Yukselt(); 
                    }
                }

                DegerleriGuncelle(); 
                pnlOyunAlani.Invalidate(); 
            }
            else
            {
                MessageBox.Show(toplamMaliyet == 0 ? "Yükseltilecek kule yok veya hepsi Max Seviye!" : $"Yetersiz Altın! Gerekli: {toplamMaliyet}");
            }
        }
    }




}
