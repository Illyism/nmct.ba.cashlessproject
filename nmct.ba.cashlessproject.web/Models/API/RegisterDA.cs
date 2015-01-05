using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.web.api.Models
{
    public class RegisterDA
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

        private static Register BuildModel(DbDataReader reader)
        {
            return new Register()
            {
                ID = int.Parse(reader["ID"].ToString()),
                RegisterName = reader["RegisterName"].ToString(),
                Device = reader["Device"].ToString()
            };
        }


        public static List<Register> GetRegisters(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Register WHERE Hidden=0;");
            List<Register> list = new List<Register>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static List<RegisterEmployee> GetRegisterEmployees(int id, IEnumerable<Claim> claims)
        {
            string sql = "SELECT r.*, e.EmployeeName, e.Address, e.Email, e.Phone FROM Register_Employee r INNER JOIN Employee e ON e.ID=r.EmployeeID WHERE r.RegisterID = @RegisterID AND r.Hidden=0 AND e.Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterID", id);
            DbDataReader reader = Database.GetData(GetConnection(claims), sql, par1);
            List<RegisterEmployee> list = new List<RegisterEmployee>();
            while (reader.Read()) list.Add(new RegisterEmployee(reader));
            reader.Close();
            return list;
        }

        public static int InsertRegEmp(RegisterEmployee regemp, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Register_Employee VALUES(@RegisterID,@EmployeeID,@FromTime,@UntilTime, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterID", regemp.RegisterID);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@EmployeeID", regemp.Employee.ID);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@FromTime", regemp.FromTime);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@UntilTime", regemp.UntilTime);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4);
        }
    }
}