namespace RFID_Scanner.DatabaseCL
{
    public class DbPostgres : Database
    {
        public static DbPostgres Instance
        {
            get
            {
                return _instance ?? (_instance = new DbPostgres("ux4.dvs-plattling.de", "db_alehner", "alehner", "alehner"));
               // return _instance ?? (_instance = new DbPostgres("localhost", "postgres", "postgres", "postgres"));
            }
        }
        private static DbPostgres _instance;

        public DbPostgres(string host, string database, string username, string password)
        : base(Type.PostgreSQL, $"Host={host};Database={database};Username={username};Password={password}")
        {
        }
    }
}
