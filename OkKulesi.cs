using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public class OkKulesi : Kule
    {

        public OkKulesi(Point konum)
        : base(konum, hasar: 25, menzil: 120, saldiriHizi: 0.5f, fiyat: 100)
        {
            GorselNesne.BackColor = Color.Blue; 
            GorselNesne.Image = Properties.Resources.OkKulesi1;
            GorselNesne.SizeMode = PictureBoxSizeMode.StretchImage;
            GorselNesne.BackColor = Color.Transparent;
        }

        // Polymorphism: TEK HEDEF (En yakın)
        public override void Saldir(List<Dusman> hedefDusmanlar)
        {
            
            List<Dusman> menzilIci = hedefDusmanlar.FindAll(d => MenzilIcindeMi(d));

            if (menzilIci.Count > 0)
            {
               
                Dusman hedef = menzilIci[0];

                
                hedef.Can -= this.Hasar;
                
            }
        }

        public override void Yukselt()
        {
            
            base.Yukselt();

           
            if (Seviye == 3)
            {
                
                this.GerekliTickSayisi = Math.Max(1, this.GerekliTickSayisi / 2);
            }
        }
    }
}
