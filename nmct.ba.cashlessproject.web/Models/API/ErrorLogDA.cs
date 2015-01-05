using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.web.Models.API
{
    public class ErrorLogDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", ".", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        private static DbConnection GetConnection(IEnumerable<Claim> claims)
        {
            return Database.GetConnection(CreateConnectionString(claims));
        }

        private static ErrorLog BuildModel(DbDataReader reader)
        {
            return new ErrorLog()
            {
                RegisterID = int.Parse(reader["RegisterID"].ToString()),
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                Message = reader["Message"].ToString(),
                Stacktrace = reader["Stacktrace"].ToString()
            };
        }

        public static List<ErrorLog> GetErrorLogs()
        {
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from Errorlog WHERE Hidden=0 ORDER BY Timestamp DESC");
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static List<ErrorLog> GetErrorLogs(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Errorlog WHERE Hidden=0");
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static List<ErrorLog> GetErrorLogRegister(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Errorlog WHERE Hidden=0");
            List<ErrorLog> list = new List<ErrorLog>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static int InsertErrorLog(ErrorLog err, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Errorlog VALUES(@RegisterID,@Timestamp,@Message,@Stacktrace, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterID", err.RegisterID);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Timestamp", DateTime.Now);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Message", err.Message);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Stacktrace", err.Stacktrace);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4);
        }

        public static void Sync()
        {
            List<Organisation> orgs = OrganisationDA.GetOrganisations();
            List<ErrorLog> errors = new List<ErrorLog>();
            foreach (Organisation eorg in orgs) {
                Organisation org = OrganisationDA.DecryptOrganisation(eorg);
                string sql = "INSERT CashlessAdmin.dbo.Errorlog SELECT * from " + org.DbName + ".dbo.Errorlog other WHERE NOT EXISTS (SELECT * FROM CashlessAdmin.dbo.Errorlog WHERE Timestamp = other.Timestamp)";
                Database.InsertData("CashlessAdmin", sql);
            }
            
        }
    }
}