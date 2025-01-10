namespace RFID_Scanner.DatabaseCL
{
    public class Aktion_akt
    {
        public int IiD { get; set; }
        public string SBezeichnung { get; set; }
        public int St_IiD { get; set; }
        public int Ver_IiD { get; set; }

        //
        public Aktion_akt() { }


        // die machst du
        public Aktion_akt(int iiD, string sBezeichnung, int st_IiD, int ver_IiD)
        {
            IiD = iiD;
            SBezeichnung = sBezeichnung;
            St_IiD = st_IiD;
            Ver_IiD = ver_IiD;
        }
        public Aktion_akt(string sBezeichnung, int st_IiD, int ver_IiD)
        {
            SBezeichnung = sBezeichnung;
            St_IiD = st_IiD;
            Ver_IiD = ver_IiD;
        }
    }
}
