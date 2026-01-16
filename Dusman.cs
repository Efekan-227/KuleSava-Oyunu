using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public class Dusman
    {
        // Encapsulation (Kapsülleme) gereksinimi: Private alanlar, Public özellikler 

        // Temel Özellikler
        public int Can { get; set; } 
        public float Hiz { get; set; } 
        public int AltinDegeri { get; set; } 

        // Konum ve Hareket Bilgileri
        public Point Konum { get; set; } 
        public int GidilecekHedefIndex { get; set; }
        public PictureBox gorselNesne;

        // Kurucu Metot (Constructor)
        public Dusman(Point baslangicKonumu, int can, float hiz, int altin, Color renk)
        {
            Can = can;
            Hiz = hiz;
            AltinDegeri = altin;
            Konum = baslangicKonumu;
            GidilecekHedefIndex = 1;

            gorselNesne = new PictureBox
            {
                Size = new Size(20, 20), 
                BackColor = renk, 
                Location = baslangicKonumu,
                Visible = true,
                BorderStyle = BorderStyle.FixedSingle,
            };

            gorselNesne.Image = Properties.Resources.DusmanResim;
            gorselNesne.SizeMode = PictureBoxSizeMode.StretchImage;
            gorselNesne.BackColor = Color.Transparent;
        }

        // Düşman nesnesini forma eklemek için metod
        public PictureBox GetGorselNesne()
        {
            return gorselNesne;

        }

        // Hareket Metodu 
        public void HareketEt(List<Point> yolKoordinatlari)
        {
            // Yolu tamamladıysa
            if (GidilecekHedefIndex >= yolKoordinatlari.Count)
            {
                return;
            }

            // Hedef Koordinat
            Point hedefNokta = yolKoordinatlari[GidilecekHedefIndex];

            int deltaX = hedefNokta.X - Konum.X;
            int deltaY = hedefNokta.Y - Konum.Y;

            // Uzaklığı 
            double uzaklik = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (uzaklik <= Hiz)
            {
                
                Konum = hedefNokta; 
                GidilecekHedefIndex++;
                gorselNesne.Location = Konum;
                return;
            }

            float birimX = (float)(deltaX / uzaklik);
            float birimY = (float)(deltaY / uzaklik);
            float yeniX = Konum.X + birimX * Hiz;
            float yeniY = Konum.Y + birimY * Hiz;
            Konum = new Point((int)yeniX, (int)yeniY);
            gorselNesne.Location = Konum;
        }
    }
}
