using System;

namespace RFID_Scanner.DatabaseCL
{
    public
        class Ver_St_Akt_ZwischenTabelle
    {
        public string VBezeichnung { get; set; }
        public string SBezeichnung { get; set; }
        public string ABezeichnung { get; set; }
        public long Besucher { get; set; }
        public string sbez { get; set; }
        public DateTime dat { get; set; }

        public Ver_St_Akt_ZwischenTabelle(string vBezeichnung, string sBezeichnung, string aBezeichnung)
        {
            VBezeichnung = vBezeichnung;
            SBezeichnung = sBezeichnung;
            ABezeichnung = aBezeichnung;
        }

        public Ver_St_Akt_ZwischenTabelle(string vBezeichnung, string sBezeichnung, string aBezeichnung, long besucher)
        {
            VBezeichnung = vBezeichnung;
            SBezeichnung = sBezeichnung;
            ABezeichnung = aBezeichnung;
            Besucher = besucher;
        }
        public Ver_St_Akt_ZwischenTabelle()
        {
        }

        public Ver_St_Akt_ZwischenTabelle(string bez, DateTime da)
        {
            sbez = bez;
            dat = da;
        }
    }
}
