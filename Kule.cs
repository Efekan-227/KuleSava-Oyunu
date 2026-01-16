using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public abstract class Kule : ISaldirabilir, IYukseltilebilir
    {
        private int _hasar;
        private int _fiyat;

        
        public int Hasar
        {
            get { return _hasar; }
            protected set { _hasar = value; } 
        }
        public int Menzil { get; protected set; }
        public float SaldiriHizi { get; protected set; } 
        public int Fiyat
        {
            get { return _fiyat; }
            protected set { _fiyat = value; }
        }

        public Point Konum { get; protected set; }
        public PictureBox GorselNesne { get; protected set; }

        
        public int SaldiriSayaci { get; set; } = 0; 
        public int GerekliTickSayisi { get; protected set; } 

        // Kurucu Metot (Constructor)
        public Label SeviyeEtiketi { get; protected set; }
        public Kule(Point konum, int hasar, int menzil, float saldiriHizi, int fiyat)
        {
            Konum = konum;
            _hasar = hasar;
            Menzil = menzil;
            SaldiriHizi = saldiriHizi;
            _fiyat = fiyat;

            SeviyeEtiketi = new Label
            {
                Text = "Lv. 1",
                ForeColor = Color.White,
                BackColor = Color.Black, 
                AutoSize = true,
                Location = new Point(konum.X, konum.Y + 42), 
                Font = new Font("Arial", 8, FontStyle.Bold)
            };

          
            GerekliTickSayisi = (int)(saldiriHizi * 10);

            
            GorselNesne = new PictureBox
            {
                Size = new Size(40, 40),
                Location = Konum,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = this
               
            };
        }

        // metod
        public abstract void Saldir(List<Dusman> hedefDusmanlar);

        // Ortak Yardımcı Metot: Menzil Kontrolü
        public bool MenzilIcindeMi(Dusman dusman)
        {
           
            double mesafe = Math.Sqrt(
                Math.Pow(dusman.Konum.X - Konum.X, 2) +
                Math.Pow(dusman.Konum.Y - Konum.Y, 2)
            );
            return mesafe <= Menzil;
        }

        // Yükseltme metotları 
        /* public virtual void Yukselt()
         {
            
             this.Hasar = (int)(this.Hasar * 1.5);
             this.Menzil = (int)(this.Menzil * 1.1);
         }
         public int Seviye { get; protected set; } = 1; 
         public virtual int YukseltmeMaliyeti => Seviye * 100; 
         public abstract void Yukselt();
        */

        public int Seviye { get; protected set; } = 1; 
        public virtual int YukseltmeMaliyeti => Seviye * 70;
        public virtual void Yukselt()
        {
            if (Seviye < 3)
            {
               
                this.Hasar = (int)(this.Hasar * 1.2);
                this.Menzil = (int)(this.Menzil * 1.1);
                Seviye++;
                SeviyeEtiketi.Text = "Lv. " + Seviye;
                if (Seviye == 3) SeviyeEtiketi.ForeColor = Color.Gold;
            }
        }

        public bool YukseltmeYapilabilir(int mevcutAltin)
        {
            return Seviye < 3 && mevcutAltin >= YukseltmeMaliyeti;
        }

    }
}
