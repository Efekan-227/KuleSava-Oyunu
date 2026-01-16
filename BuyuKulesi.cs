using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public class BuyuKulesi : Kule 
    {

        public BuyuKulesi(Point konum)
        : base(konum, hasar: 20, menzil: 130, saldiriHizi: 1.5f, fiyat: 200)
        {
            GorselNesne.BackColor = Color.Purple; 
                                                  
            GorselNesne.Image = Properties.Resources.BüyücüKulesi;
            GorselNesne.SizeMode = PictureBoxSizeMode.StretchImage;
            GorselNesne.BackColor = Color.Transparent;
        }

        // Polymorphism: ÇOKLU HEDEF (En yakın 5 düşman)
        public override void Saldir(List<Dusman> hedefDusmanlar)
        {
            
            List<Dusman> menzilIci = hedefDusmanlar.FindAll(d => MenzilIcindeMi(d));

            if (menzilIci.Count > 0)
            {
                
                var hedefler = menzilIci.OrderBy(d => (d.Konum.X - Konum.X) * (d.Konum.X - Konum.X) + (d.Konum.Y - Konum.Y) * (d.Konum.Y - Konum.Y))
                                         .Take(5).ToList();

                // 3. Hedeflere hasar ver
                foreach (Dusman hedef in hedefler)
                {
                    hedef.Can -= this.Hasar;
                }
            }
        }

        public override void Yukselt()
        {
            base.Yukselt(); 

            if (Seviye == 3)
            {
                
                this.Menzil = (int)(this.Menzil * 1.1);
            }
        }
    }
}
