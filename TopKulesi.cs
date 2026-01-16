using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public class TopKulesi : Kule
    {

        public TopKulesi(Point konum)
        : base(konum, hasar: 50, menzil: 140, saldiriHizi: 2.5f, fiyat: 220)
        {
            GorselNesne.BackColor = Color.Orange; 
            GorselNesne.Image = Properties.Resources.TopKulesi;
            GorselNesne.SizeMode = PictureBoxSizeMode.StretchImage;
            GorselNesne.BackColor = Color.Transparent;
        }

        // Polymorphism: ÇOKLU HEDEF 
        public override void Saldir(List<Dusman> hedefDusmanlar)
        {
            List<Dusman> menzilIci = hedefDusmanlar.FindAll(d => MenzilIcindeMi(d));

            if (menzilIci.Count > 0)
            {
                foreach (Dusman hedef in menzilIci)
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
