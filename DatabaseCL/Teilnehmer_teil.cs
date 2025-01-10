using System;

namespace RFID_Scanner.DatabaseCL
{
    public class Teilnehmer_teil
    {
        public string Chip_ID { get; set; }
        public int Aktions_ID { get; set; }
        public DateTime Zeitstempel { get; set; }

        public Teilnehmer_teil(string chip_ID, int aktions_ID, DateTime zeitstempel)
        {
            Chip_ID = chip_ID;
            Aktions_ID = aktions_ID;
            Zeitstempel = zeitstempel;
        }
        public Teilnehmer_teil()
        {
        }
        public Teilnehmer_teil(string chip_ID, DateTime zeitstempel)
        {
            Chip_ID = chip_ID;
            Zeitstempel = zeitstempel;
        }
    }
}
