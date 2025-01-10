using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RFID_Scanner.DatabaseCL
{
    public class Database
    {

        private string ConnectionString { get; set; }
        public Type DbType { get; private set; }

        public enum Type { Unknown, PostgreSQL }

        protected Database(Type dbType, string connectionString)
        {
            ConnectionString = connectionString;
            DbType = dbType;
        }

        private DbConnection NewConnection()
        {
            switch (DbType)
            {
                case Type.PostgreSQL:
                    return new NpgsqlConnection(ConnectionString);
                default:
                    throw new ArgumentException("Unknown Database Server Type");
            }
        }

       
    public bool CheckIfGoodDs(Aktion_akt ZuPruefendeAktion)
        {
            Aktion_akt Benni = new Aktion_akt();
            int id = Benni.IiD;
            List<Aktion_akt> Aktion = new List<Aktion_akt>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT sbezeichnung, s_id, v_id FROM Aktion_a WHERE s_id = @p1 AND v_id = @p2 AND sbezeichnung = @p3 ";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = ZuPruefendeAktion.St_IiD;
                cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = ZuPruefendeAktion.Ver_IiD;
                cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter();
                p3.ParameterName = "@p3";
                p3.Value = ZuPruefendeAktion.SBezeichnung;
                cmd.Parameters.Add(p3);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sBezeichnung = reader.GetString(0);
                        int st_Iid = reader.GetInt32(1);
                        int ver_Iid = reader.GetInt32(2);
                        Aktion.Add(new Aktion_akt(sBezeichnung, st_Iid, ver_Iid));
                    }
                }
            }
            return Aktion.Count > 0;
        }
        public List<Aktion_akt> GetAktion(string Standort)
        {
            List<Aktion_akt> Aktion = new List<Aktion_akt>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid,sbezeichnung,s_id,v_id FROM Aktion_a WHERE s_id = @p1  ";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = GetAktionZuStandort(Standort);
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Iid = reader.GetInt32(0);
                        string sBezeichnung = reader.GetString(1);
                        int st_Iid = reader.GetInt32(2);
                        int ver_Iid = reader.GetInt32(3);
                        Aktion.Add(new Aktion_akt(Iid, sBezeichnung, st_Iid, ver_Iid));
                    }
                }
            }
            return Aktion;
        }
        public Standort_st GetStandortmitKoords(string sbezeichnung)
        {
            Standort_st Standort = new Standort_st();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid, sbezeichnung, xkoordinate, ykoordinate, OberUntergeschoss FROM Standort_s WHERE sbezeichnung = @p1";

                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = sbezeichnung;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Iid = reader.GetInt32(0);
                        string sBezeichnung = reader.GetString(1);
                        double x_ko = reader.GetDouble(2);
                        double y_ko = reader.GetDouble(3);
                        char UnterOber = reader.GetChar(4);
                        Standort = new Standort_st(Iid, sBezeichnung, x_ko, y_ko, UnterOber);
                    }
                }
            }
            return Standort;
        }
        public List<Standort_st> GetStandort()
        {
            List<Standort_st> Standort = new List<Standort_st>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid, sbezeichnung  FROM Standort_s";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Iid = reader.GetInt32(0);
                        string sBezeichnung = reader.GetString(1);
                        Standort.Add(new Standort_st(Iid, sBezeichnung));
                    }
                }
            }
            return Standort;
        }

        public int GetAktionsId(string sbezeichnung)
        {
            int AktionsId = -1;
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid,sbezeichnung,s_id,v_id FROM Aktion_a where sbezeichnung = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = sbezeichnung;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AktionsId = reader.GetInt32(0);
                    }
                }
            }
            return AktionsId;
        }

        public List<Aktion_akt> GetAktion()
        {
            List<Aktion_akt> Aktion = new List<Aktion_akt>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid,sbezeichnung,s_id,v_id FROM Aktion_a";
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Iid = reader.GetInt32(0);
                        string sBezeichnung = reader.GetString(1);
                        int st_Iid = reader.GetInt32(2);
                        int ver_Iid = reader.GetInt32(3);
                        Aktion.Add(new Aktion_akt(Iid, sBezeichnung, st_Iid, ver_Iid));
                    }
                }
            }
            return Aktion;
        }
        public List<Standort_st> Standortliste(List<int> ids)
        {
            List<Standort_st> st_ids = new List<Standort_st>();
            foreach (var item in ids)
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "SELECT iid, sbezeichnung, xkoordinate, ykoordinate, OberUntergeschoss FROM standort_s WHERE IID = @p1";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = item;
                    cmd.Parameters.Add(p1);
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            st_ids.Add(new Standort_st(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetDouble(3), reader.GetChar(4)));
                        }
                    }
                }
            }
            return st_ids;
        }

        public List<int> StandortIDS(int v_id)
        {
            List<int> st_ids = new List<int>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT s_id FROM aktion_a WHERE v_id = @p1 group by s_id";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = v_id;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int iid = reader.GetInt32(0);
                        st_ids.Add(iid);
                    }
                }
            }
            return st_ids;
        }
        public int GetVeranstaltungsID(string name)
        {
            List<int> Veranstalungen = new List<int>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT IID FROM veranstaltung_v WHERE sbezeichnung = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = name;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Veranstalungen.Add(reader.GetInt32(0));
                    }
                }
            }
            return Veranstalungen.Count > 0 ? Veranstalungen[0] : -1;
        }

        public bool CheckMAsertercard(string chipnr)
        {
            List<long> Besucher = new List<long>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT count(*) FROM Mastercard_ma WHERE chipnr = @p1";

                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = chipnr;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;

                Int64 count = (Int64)cmd.ExecuteScalar();
                Besucher.Add(count);
                con.Close();
            }
            return Besucher[0] <= 0;
        }
        public bool InsertMastercard(string chipnr)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "Insert into Mastercard_ma (chipnr) VALUES (@p1)";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = chipnr;
                    cmd.Parameters.Add(p1);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetAktionZuStandort(string sbezeichnung)
        {
            List<int> Veranstalungen = new List<int>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT IID FROM standort_s WHERE sbezeichnung = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = sbezeichnung;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Veranstalungen.Add(reader.GetInt32(0));
                    }
                }
                con.Close();
            }
            return Veranstalungen.Count > 0 ? Veranstalungen[0] : -1;
        }

        public int GetStandortZuVeranstaltung(string sbezeichnung)
        {
            List<int> Veranstalungen = new List<int>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT IID FROM standort_s WHERE sbezeichnung = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = sbezeichnung;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Veranstalungen.Add(reader.GetInt32(0));
                    }
                }
                con.Close();
            }
            return Veranstalungen[0];
        }
        public List<Veranstaltung_ver> GetVeranstaltungen()
        {
            List<Veranstaltung_ver> Veranstalungen = new List<Veranstaltung_ver>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid,sbezeichnung FROM Veranstaltung_v";

                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Iid = reader.GetInt32(0);
                        string sBezeichnung = reader.GetString(1);
                        Veranstalungen.Add(new Veranstaltung_ver(Iid, sBezeichnung));
                    }
                }
                con.Close();
            }
            return Veranstalungen;
        }

        public bool InsertKurs(Aktion_akt NeueAktion)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "Insert into Aktion_a (sbezeichnung, v_id, s_id, sCreator) VALUES (@p1, @p2, @p3,@p4)";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = NeueAktion.SBezeichnung;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = NeueAktion.Ver_IiD;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = NeueAktion.St_IiD;
                    cmd.Parameters.Add(p3);
                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@p4";
                    p4.Value = Environment.UserName;
                    cmd.Parameters.Add(p4);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteKurs(Aktion_akt ZuLoeschendeAktioin)
        {
            try
            {
                DeleteTeilnehmervonAktion(GetAktionsId(ZuLoeschendeAktioin.SBezeichnung));
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "DELETE FROM Aktion_a WHERE sbezeichnung = @p1 AND v_id = @p2 AND s_id = @p3";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = ZuLoeschendeAktioin.SBezeichnung;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = ZuLoeschendeAktioin.Ver_IiD;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = ZuLoeschendeAktioin.St_IiD;
                    cmd.Parameters.Add(p3);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateStandort(string standortalt, string standortneu, double xneu, double yneu, char OberUntergeschoss)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "UPDATE standort_s SET sbezeichnung = @p2, xkoordinate = @p3 , ykoordinate = @p4 , OberUntergeschoss = @p5, sCreator = @p6 WHERE sbezeichnung = @p1";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = standortalt;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = standortneu;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = xneu;
                    cmd.Parameters.Add(p3);
                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@p4";
                    p4.Value = yneu;
                    cmd.Parameters.Add(p4);
                    var p5 = cmd.CreateParameter();
                    p5.ParameterName = "@p5";
                    p5.Value = OberUntergeschoss;
                    cmd.Parameters.Add(p5);
                    var p6 = cmd.CreateParameter();
                    p6.ParameterName = "@p6";
                    p6.Value = Environment.UserName;
                    cmd.Parameters.Add(p6);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertStandort(string standort, double x_koord, double y_koord, char OberUntergeschoss)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "Insert into standort_s (sbezeichnung, xkoordinate, ykoordinate,OberUntergeschoss, sCreator) VALUES (@p1, @p2, @p3, @p4, @p5)";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = standort;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = x_koord;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = y_koord;
                    cmd.Parameters.Add(p3);
                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@p4";
                    p4.Value = OberUntergeschoss;
                    cmd.Parameters.Add(p4);
                    var p5 = cmd.CreateParameter();
                    p5.ParameterName = "@p5";
                    p5.Value = Environment.UserName;
                    cmd.Parameters.Add(p5);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteStandort(string standort)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "DELETE FROM standort_s WHERE sbezeichnung = @p1";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = standort;
                    cmd.Parameters.Add(p1);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateVeranstaltung(string veranstaltungalt, string veranstaltungneu)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "UPDATE veranstaltung_v SET sbezeichnung = @p1, sCreator = @p3 WHERE sbezeichnung = @p2";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = veranstaltungneu;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = veranstaltungalt;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = Environment.UserName;
                    cmd.Parameters.Add(p3);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();

                    con.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertVeranstaltung(string veranstaltung)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "Insert into veranstaltung_v (sbezeichnung, sCreator) VALUES (@p1, @p2)";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = veranstaltung;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = Environment.UserName;
                    cmd.Parameters.Add(p2);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteVeranstaltung(string veranstaltung)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "DELETE FROM veranstaltung_v WHERE sbezeichnung = @p1";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = veranstaltung;
                    cmd.Parameters.Add(p1);
                    cmd.CommandText = sql;
                    cmd.ExecuteReader();
                    con.Close();
                }
                return true;
            }
            catch (Exception)
            {
               return false;
            }
        }
        public bool UpdateAktion(Aktion_akt OldAkt, Aktion_akt NewAkt)
        {
            try
            {
                using (var con = NewConnection())
                {
                    con.Open();
                    const string sql = "UPDATE Aktion_a SET sbezeichnung = @p1, v_id = @p2, s_id = @p3, sCreator = @p7 WHERE sbezeichnung = @p4 AND v_id = @p5 AND s_id = @p6";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = NewAkt.SBezeichnung;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = NewAkt.Ver_IiD;
                    cmd.Parameters.Add(p2);
                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@p3";
                    p3.Value = NewAkt.St_IiD;
                    cmd.Parameters.Add(p3);
                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@p4";
                    p4.Value = OldAkt.SBezeichnung;
                    cmd.Parameters.Add(p4);
                    var p5 = cmd.CreateParameter();
                    p5.ParameterName = "@p5";
                    p5.Value = OldAkt.Ver_IiD;
                    cmd.Parameters.Add(p5);
                    var p6 = cmd.CreateParameter();
                    p6.ParameterName = "@p6";
                    p6.Value = OldAkt.St_IiD;
                    cmd.Parameters.Add(p6);
                    var p7 = cmd.CreateParameter();
                    p7.ParameterName = "@p7";
                    p7.Value = Environment.UserName;
                    cmd.Parameters.Add(p7);
                    cmd.CommandText = sql;

                    cmd.ExecuteReader();
                    con.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckInsert(Teilnehmer_teil t1)
        {
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT chipnr, zeitstempel, a_id FROM Teilnehmer_t";
                List<Teilnehmer_teil> TeilnehmerListe = new List<Teilnehmer_teil>();
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string Chip_id = reader.GetString(0);
                        DateTime sBezeichnung = reader.GetDateTime(1);
                        int Aktions_id = reader.GetInt32(2);
                        TeilnehmerListe.Add(new Teilnehmer_teil(Chip_id, Aktions_id, sBezeichnung));
                    }
                }
                con.Close();
                foreach (var _ in from Teilnehmer_teil item in TeilnehmerListe
                                  where item.Chip_ID == t1.Chip_ID && item.Aktions_ID == t1.Aktions_ID
                                  select new { })
                {
                    return false;
                }

                return true;
            }
        }
        public void InsertTeilnehmer(Teilnehmer_teil t1)
        {
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "Insert into Teilnehmer_t (chipnr, zeitstempel, a_id) VALUES (@p1, @p3, @p2)";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = t1.Chip_ID.Trim();
                cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = t1.Aktions_ID;
                cmd.Parameters.Add(p2);
                var p3 = cmd.CreateParameter();
                p3.ParameterName = "@p3";
                p3.Value = t1.Zeitstempel;
                cmd.Parameters.Add(p3);
                cmd.CommandText = sql;
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public void DeleteTeilnehmervonAktion(int iid)
        {
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "DELETE FROM Teilnehmer_t WHERE a_id = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = iid;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public List<Teilnehmer_teil> GetTeilnehmerZuAktID(int id)
        {
            List<Teilnehmer_teil> Veranstalungen = new List<Teilnehmer_teil>();
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT chipnr, zeitstempel FROM Teilnehmer_t WHERE a_id = @p1 ORDER BY zeitstempel DESC";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = id;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string Chip_Nr = reader.GetString(0);
                        DateTime Zeitstempel = reader.GetDateTime(1);
                        Veranstalungen.Add(new Teilnehmer_teil(Chip_Nr, Zeitstempel));
                    }
                }
                con.Close();
            }
            return Veranstalungen;
        }
        public void TeilnehmerLoeschen(string chip_nr, DateTime dateTime)
        {
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "DELETE FROM Teilnehmer_t WHERE chipnr = @p1 AND zeitstempel = @p2";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = chip_nr;
                cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = dateTime;
                cmd.Parameters.Add(p2);
                cmd.CommandText = sql;
                cmd.ExecuteReader();
                con.Close();
            }
        }
        public int AktionsIDvonName(string name)
        {
            int IId = -1;
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT iid FROM Aktion_a WHERE sbezeichnung = @p1";

                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = name;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        IId = reader.GetInt32(0);
                    }
                }
                con.Close();
            }
            return IId;
        }

        public List<DateTime> GetChartContent(string AktionsName)
        {
            List<DateTime> Uhrzeiten = new List<DateTime>();
            int Aktions_id = AktionsIDvonName(AktionsName);
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT zeitstempel FROM Teilnehmer_t WHERE a_id = @p1";

                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = Aktions_id;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Uhrzeiten.Add(reader.GetDateTime(0));
                    }
                }
                con.Close();
            }
            return Uhrzeiten;
        }

        public long CountBesucher(string aktion)
        {
            List<long> Besucher = new List<long>();
            using (var con = NewConnection())
            {
                int Aktions_id = AktionsIDvonName(aktion);
                con.Open();
                const string sql = "SELECT count(*) FROM Teilnehmer_t WHERE a_id = @p1";

                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = Aktions_id;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;

                Int64 count = (Int64)cmd.ExecuteScalar();
                Besucher.Add(count);
                con.Close();
            }
            return Besucher[0];
        }

        public long CountBesuchermitZeit(string aktion, int vuhr, int buhr)
        {
            List<long> Besucher = new List<long>();
            using (var con = NewConnection())
            {
                int Aktions_id = AktionsIDvonName(aktion);
                con.Open();
                const string sql = "Select Count(*) from Teilnehmer_t as t where EXTRACT(hour FROM zeitstempel) < @p2 and EXTRACT(hour FROM zeitstempel) >= @p3 and a_id = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                var p2 = cmd.CreateParameter();
                var p3 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p2.ParameterName = "@p2";
                p3.ParameterName = "@p3";
                p1.Value = Aktions_id;
                p2.Value = buhr;
                p3.Value = vuhr;
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.CommandText = sql;

                Int64 count = (Int64)cmd.ExecuteScalar();
                Besucher.Add(count);
                con.Close();
            }
            return Besucher[0];
        }

        public Dictionary<string, DateTime> GetTeilnehmervonAktion(string chipnr, string veranstaltung)
        {
            Dictionary<string, DateTime> AktimitZeit = new Dictionary<string, DateTime>();
            int v_id = GetVeranstaltungsID(veranstaltung);
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT a.sbezeichnung, t.zeitstempel FROM aktion_a as a join teilnehmer_t as t on t.a_id = a.iId WHERE chipnr = @p1 AND v_id = @p2 order by a.sbezeichnung";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = chipnr;
                cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = v_id;
                cmd.Parameters.Add(p2);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sbez = reader.GetString(0);
                        DateTime Zeitstempel = reader.GetDateTime(1);
                        AktimitZeit.Add(sbez, Zeitstempel);
                    }
                }
                con.Close();
            }
            return AktimitZeit;
        }
        public List<Standort_st> GetTeilnehmervonAktionMitKoordinaten(string chipnr, string veranstaltungsname)
        {
            List<Standort_st> AktionenMitKoordinaten = new List<Standort_st>();
            using (var con = NewConnection())
            {
                int v_id = GetVeranstaltungsID(veranstaltungsname);
                con.Open();
                const string sql = "SELECT a.sbezeichnung, xkoordinate, ykoordinate, OberUntergeschoss,t.Zeitstempel,s.sbezeichnung FROM aktion_a as a join teilnehmer_t as t on t.a_id = a.iId join standort_s as s on s.iid = a.s_id  WHERE chipnr = @p1 AND a.v_id = @p2 ";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = chipnr;
                cmd.Parameters.Add(p1);
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = v_id;
                cmd.Parameters.Add(p2);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sbez = reader.GetString(0);
                        double Xkoordinate = reader.GetDouble(1);
                        double Ykoordinate = reader.GetDouble(2);
                        char UnterOber = reader.GetChar(3);
                        DateTime ZeitStempel = reader.GetDateTime(4);
                        string Sbezeichnung = reader.GetString(5);
                        AktionenMitKoordinaten.Add(new Standort_st(sbez, Xkoordinate, Ykoordinate, UnterOber, ZeitStempel, Sbezeichnung));
                    }
                }
                con.Close();
            }
            return AktionenMitKoordinaten;
        }
        public List<Standort_st> AlleStandorteZuAktion(List<Aktion_akt> Aktionen, string Veranstaltungsname)
        {
            List<Standort_st> AktionenMitKoordinaten = new List<Standort_st>();
            int V_id = GetVeranstaltungsID(Veranstaltungsname);
            using (var con = NewConnection())
            {
                con.Open();
                foreach (var item in Aktionen)
                {
                    const string sql = "SELECT a.sbezeichnung, xkoordinate, ykoordinate, OberUntergeschoss, s.sbezeichnung FROM aktion_a as a join standort_s as s on s.iid = a.s_id  WHERE a.iid = @p1 and a.v_id = @p2";
                    var cmd = con.CreateCommand();
                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@p1";
                    p1.Value = item.IiD;
                    cmd.Parameters.Add(p1);
                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@p2";
                    p2.Value = V_id;
                    cmd.Parameters.Add(p2);
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sbez = reader.GetString(0);
                            double Xkoordinate = reader.GetDouble(1);
                            double Ykoordinate = reader.GetDouble(2);
                            char UnterOber = reader.GetChar(3);
                            string sbezeichnung = reader.GetString(4);
                            AktionenMitKoordinaten.Add(new Standort_st(sbez, Xkoordinate, Ykoordinate, UnterOber, sbezeichnung));
                        }
                    }
                }

                con.Close();
            }
            return AktionenMitKoordinaten;
        }
        public string GetVeranstaltungvonTeilnehmer(string chipnr)
        {
            string sbez = "";
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "select v.sbezeichnung from teilnehmer_t as t join aktion_a as a on a.iId = t.a_id join veranstaltung_v as v on v.iId = a.v_id where t.chipnr = @p1";
                var cmd = con.CreateCommand();
                var p1 = cmd.CreateParameter();
                p1.ParameterName = "@p1";
                p1.Value = chipnr;
                cmd.Parameters.Add(p1);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sbez = reader.GetString(0);
                    }
                }
                con.Close();
            }
            return sbez;
        }
        public List<Standort_st> AlleAktionenFürVeranstaltung(string Veranstaltungsname)
        {
            List<Standort_st> AktionenMitKoordinaten = new List<Standort_st>();
            int V_id = GetVeranstaltungsID(Veranstaltungsname);
            using (var con = NewConnection())
            {
                con.Open();
                const string sql = "SELECT a.sbezeichnung, xkoordinate, ykoordinate, OberUntergeschoss FROM aktion_a as a join standort_s as s on s.iid = a.s_id  WHERE a.v_id = @p2";
                var cmd = con.CreateCommand();
                var p2 = cmd.CreateParameter();
                p2.ParameterName = "@p2";
                p2.Value = V_id;
                cmd.Parameters.Add(p2);
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string sbez = reader.GetString(0);
                        double Xkoordinate = reader.GetDouble(1);
                        double Ykoordinate = reader.GetDouble(2);
                        char UnterOber = reader.GetChar(3);
                        AktionenMitKoordinaten.Add(new Standort_st(sbez, Xkoordinate, Ykoordinate, UnterOber));
                    }
                }
                con.Close();
            }
            return AktionenMitKoordinaten;
        }
    }
}
