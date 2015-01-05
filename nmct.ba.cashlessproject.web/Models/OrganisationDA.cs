using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.web.Models
{
    public class OrganisationDA
    {
        public static Organisation CheckCredentials(string user, string pass)
        {
            string sql = "SELECT * FROM Organisations WHERE Login=@login AND Password=@pass AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@login", Cryptography.Encrypt(user));
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@pass", Cryptography.Encrypt(pass));
            try
            {
                DbDataReader r = Database.GetData("CashlessAdmin", sql, par1, par2);
                r.Read();
                return BuildModel(r);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public static List<string> GetOrganisationNames()
        {
            string sql = "SELECT OrganisationName FROM Organisations WHERE Hidden=0";
            List<string> orgs = new List<string>();
        
            DbDataReader r = Database.GetData("CashlessAdmin", sql);
            while (r.Read()) orgs.Add(r["OrganisationName"].ToString());

            return orgs;
        }

        private static Organisation BuildModel(DbDataReader reader)
        {
            return new Organisation()
            {
                ID = Int32.Parse(reader["ID"].ToString()),
                Login = reader["Login"].ToString(),
                Password = reader["Password"].ToString(),
                DbName = reader["DbName"].ToString(),
                DbLogin = reader["DbLogin"].ToString(),
                DbPassword = reader["DbPassword"].ToString(),
                OrganisationName = reader["OrganisationName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString()
            };
        }

        public static bool MakeDB(Organisation org)
        {
            try
            {
                string sql = "RESTORE DATABASE @DbName FROM DISK = @DbLocation WITH RECOVERY, MOVE 'CashlessTemplate' TO  @DbRowsLocation, MOVE 'CashlessTemplate_Log' TO @DbLogsLocation;";

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string DbLocation = baseDir + "DB\\CashlessTemplate.bak";
                string DbRowsLocation = baseDir + "DB\\" + org.DbName + "_Data.mdf";
                string DbLogsLocation = baseDir + "DB\\" + org.DbName + "_Log.mdf";

                DbParameter par1 = Database.AddParameter("CashlessAdmin", "@DbName", org.DbName);
                DbParameter par2 = Database.AddParameter("CashlessAdmin", "@DbLocation", DbLocation);
                DbParameter par3 = Database.AddParameter("CashlessAdmin", "@DbRowsLocation", DbRowsLocation);
                DbParameter par4 = Database.AddParameter("CashlessAdmin", "@DbLogsLocation", DbLogsLocation);

                Database.ModifyData("CashlessAdmin", sql, par1, par2, par3, par4);

                MakeUser(org);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static void MakeUser(Organisation org)
        {
            ConnectionStringSettings settings = Database.CreateConnectionString(org.DbName);

            string sql1 = "ALTER DATABASE CURRENT SET CONTAINMENT = PARTIAL";
            string sql2 = "CREATE USER @@DbLogin WITH PASSWORD='@@DbPassword'";
            string sql3 = "GRANT CONNECT, SELECT, UPDATE, DELETE, INSERT TO @@DbLogin";

            sql2 = sql2.Replace("@@DbLogin", org.DbLogin).Replace("@@DbPassword", org.DbPassword);
            sql3 = sql3.Replace("@@DbLogin", org.DbLogin);

            Database.ModifyData(Database.GetConnection(settings), sql1);
            Database.ModifyData(Database.GetConnection(settings), sql2);
            Database.ModifyData(Database.GetConnection(settings), sql3);
        }

        public static List<Organisation> GetOrganisations()
        {
            string sql = "SELECT * FROM Organisations WHERE Hidden=0";
            List<Organisation> orgs = new List<Organisation>();

            DbDataReader r = Database.GetData("CashlessAdmin", sql);
            while (r.Read()) orgs.Add(BuildModel(r));

            return orgs;
        }

        public static Organisation GetOrganisation(int id)
        {
            string sql = "SELECT * FROM Organisations WHERE Id=@id AND Hidden=0";
            DbParameter par = Database.AddParameter("CashlessAdmin", "id", id);
            List<Organisation> orgs = new List<Organisation>();

            DbDataReader r = Database.GetData("CashlessAdmin", sql, par);
            r.Read();
            return BuildModel(r);
        }

        public static Organisation DecryptOrganisation(Organisation org)
        {
            return new Organisation()
            {
                ID = org.ID,
                Login = Cryptography.Decrypt(org.Login),
                Password = Cryptography.Decrypt(org.Password),
                DbName = Cryptography.Decrypt(org.DbName),
                DbLogin = Cryptography.Decrypt(org.DbLogin),
                DbPassword = Cryptography.Decrypt(org.DbPassword),
                OrganisationName = org.OrganisationName,
                Address = org.Address,
                Email = org.Email,
                Phone = org.Phone
            };
        }

        public static Organisation EncryptOrganisation(Organisation org)
        {
            return new Organisation()
            {
                ID = org.ID,
                Login = Cryptography.Encrypt(org.Login),
                Password = Cryptography.Encrypt(org.Password),
                DbName = Cryptography.Encrypt(org.DbName),
                DbLogin = Cryptography.Encrypt(org.DbLogin),
                DbPassword = Cryptography.Encrypt(org.DbPassword),
                OrganisationName = org.OrganisationName,
                Address = org.Address,
                Email = org.Email,
                Phone = org.Phone
            };
        }

        public static int InsertOrganisation(Organisation o)
        {
            string sql = "INSERT INTO Organisations VALUES(@Login,@Password,@DbName,@DbLogin,@DbPassword,@OrganisationName,@Address,@Email,@Phone, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@Login", o.Login);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Password", o.Password);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@DbName", o.DbName);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@DbLogin", o.DbLogin);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@DbPassword", o.DbPassword);
            DbParameter par6 = Database.AddParameter("CashlessAdmin", "@OrganisationName", o.OrganisationName);
            DbParameter par7 = Database.AddParameter("CashlessAdmin", "@Address", o.Address);
            DbParameter par8 = Database.AddParameter("CashlessAdmin", "@Email", o.Email);
            DbParameter par9 = Database.AddParameter("CashlessAdmin", "@Phone", o.Phone);
            return Database.InsertData("CashlessAdmin", sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);
        }

        public static void UpdateOrganisation(Organisation o)
        {
            string sql = "UPDATE Organisations SET Login=@Login, Password=@Password, DbName=@DbName, DbLogin=@DbLogin, DbPassword=@DbPassword, OrganisationName=@OrganisationName, Address=@Address, Email=@Email, Phone=@Phone WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@Login", o.Login);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Password", o.Password);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@DbName", o.DbName);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@DbLogin", o.DbLogin);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@DbPassword", o.DbPassword);
            DbParameter par6 = Database.AddParameter("CashlessAdmin", "@OrganisationName", o.OrganisationName);
            DbParameter par7 = Database.AddParameter("CashlessAdmin", "@Address", o.Address);
            DbParameter par8 = Database.AddParameter("CashlessAdmin", "@Email", o.Email);
            DbParameter par9 = Database.AddParameter("CashlessAdmin", "@Phone", o.Phone);
            DbParameter parid = Database.AddParameter("CashlessAdmin", "@ID", o.ID);
            Database.ModifyData("CashlessAdmin", sql, parid, par1, par2, par3, par4, par5, par6, par7, par8, par9);
        }

        public static void DeleteOrganisation(int id)
        {
            //string sql = "DELETE FROM Organisation WHERE ID=@ID";
            string sql = "UPDATE Organisations SET Hidden=1 WHERE ID=@ID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", id);
            Database.ModifyData("CashlessAdmin", sql, par1);
        }

        public static void ChangePassword(IEnumerable<Claim> claims, string password)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string sql = "UPDATE Organisations SET Password=@Password WHERE DbLogin=@DbLogin AND DbPassword=@DbPassword AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@Password", Cryptography.Encrypt(password));
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@DbLogin", dblogin);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@DbPassword", dbpass);
            Database.ModifyData("CashlessAdmin", sql, par1, par2, par3);
        }
    }
}