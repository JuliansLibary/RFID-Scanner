using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RFID_Scanner.LayoutCL
{
    class Aufkleber
    {
        public string Name { get; set; }
        public string PunkteLogikTest { get; set; }
        public List<string> BesuchteKurse { get; set; }
        public BitmapImage Logo { get; set; }
    }
}
