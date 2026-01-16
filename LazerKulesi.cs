using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NdpKuleSavunma
{
    public class LazerKulesi : Kule
    {
        public LazerKulesi(Point konum)
        
        : base(konum, hasar: 10, menzil: 160, saldiriHizi: 0.2f, fiyat: 400)
        {
            GorselNesne.BackColor = Color.Cyan; 
            GorselNesne.Image = Properties.Resources.LazerKulesi;
            GorselNesne.SizeMode = PictureBoxSizeMode.StretchImage;
            GorselNesne.BackColor = Color.Transparent;
        }

        // Polymorphism: ÇOK HIZLI TEK HEDEF
        public override void Saldir(List<Dusman> hedefDusmanlar)
        {
            List<Dusman> menzilIci = hedefDusmanlar.FindAll(d => MenzilIcindeMi(d));

            if (menzilIci.Count > 0)
            {
              
                Dusman hedef = menzilIci[0];
                hedef.Can -= this.Hasar;
            }
        }
    }
}
