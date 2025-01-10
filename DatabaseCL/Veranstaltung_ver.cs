namespace RFID_Scanner.DatabaseCL
{
    public class Veranstaltung_ver
    {
        public int IiD { get; set; }
        public string SBezeichnung { get; set; }

        public Veranstaltung_ver(int iiD, string sBezeichnung)
        {
            IiD = iiD;
            SBezeichnung = sBezeichnung;
        }
        public Veranstaltung_ver(string sBezeichnung)
        {
            SBezeichnung = sBezeichnung;
        }
        public Veranstaltung_ver()
        {
        }

        public override string ToString()
        {
            return IiD + SBezeichnung;
        }
    }
}
