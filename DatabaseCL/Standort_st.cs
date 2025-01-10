using System;

namespace RFID_Scanner.DatabaseCL
{
    public class Standort_st
    {
        public int IiD { get; set; }
        public string SBezeichnung { get; set; }
        public double Y_Kooridinate { get; set; }
        public double X_Kooridinate { get; set; }

        public char UnterOber { get; set; }
        public DateTime Zeitstempel { get; set; }
        public string Bezeichnung { get; set; }

        public Standort_st(string sBezeichnung, double x_Kooridinate, double y_Kooridinate, char unterOber) : this(sBezeichnung)
        {
            X_Kooridinate = x_Kooridinate;
            Y_Kooridinate = y_Kooridinate;
            UnterOber = unterOber;
        }

        public Standort_st(string sBezeichnung, double y_Kooridinate, double x_Kooridinate, char unterOber, DateTime zeitstempel, string bezeichnung) : this(sBezeichnung, y_Kooridinate, x_Kooridinate, unterOber, zeitstempel)
        {
            Bezeichnung = bezeichnung;
        }

        public Standort_st(string sBezeichnung, double y_Kooridinate, double x_Kooridinate, char unterOber, string bezeichnung) : this(sBezeichnung, y_Kooridinate, x_Kooridinate, unterOber)
        {
            Bezeichnung = bezeichnung;
        }

        public Standort_st(int iiD, string sBezeichnung, double x_Kooridinate, double y_Kooridinate, char unterOber) : this(iiD, sBezeichnung, x_Kooridinate, y_Kooridinate)
        {
            UnterOber = unterOber;
        }

        public Standort_st(int iiD, string sBezeichnung, double x_Kooridinate, double y_Kooridinate) : this(iiD, sBezeichnung)
        {
            X_Kooridinate = x_Kooridinate;
            Y_Kooridinate = y_Kooridinate;
        }

        public Standort_st(int iiD, string sBezeichnung)
        {
            IiD = iiD;
            SBezeichnung = sBezeichnung;
        }
        public Standort_st(string sBezeichnung)
        {
            SBezeichnung = sBezeichnung;
        }
        public Standort_st()
        {
        }

        public Standort_st(string sBezeichnung, double x_Kooridinate, double y_Kooridinate, char unterOber, DateTime zeitstmepel) : this(sBezeichnung, x_Kooridinate, y_Kooridinate, unterOber)
        {
            Zeitstempel = zeitstmepel;
        }
    }
}
