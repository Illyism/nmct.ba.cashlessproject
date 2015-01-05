using nmct.ba.cashlessproject.web.Helpers;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.web.api.Models
{
    public class EmployeeDA
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

        private static Employee BuildModel(DbDataReader reader)
        {
            return new Employee()
            {
                ID = int.Parse(reader["ID"].ToString()),
                EmployeeName = reader["EmployeeName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString(),
                NationalNumber = reader["NationalNumber"].ToString()
            };
        }


        public static List<Employee> GetEmployees(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Employee WHERE Hidden=0");
            List<Employee> list = new List<Employee>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static Employee GetEmployee(int id, IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Employee WHERE ID=@ID AND Hidden=0",
                Database.AddParameter("CashlessAdmin", "@ID", id));
            reader.Read();
            return BuildModel(reader);
        }

        public static Employee GetEmployeeByNationalNumber(string id, IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Employee WHERE NationalNumber=@NationalNumber AND Hidden=0",
                Database.AddParameter("CashlessAdmin", "@NationalNumber", id));
            reader.Read();
            return BuildModel(reader);
        }

        public static int InsertEmployee(Employee c, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Employee VALUES(@EmployeeName,@Address,@Email,@Phone,@NationalNumber, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@EmployeeName", c.EmployeeName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Address", c.Address);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Email", c.Email);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Phone", c.Phone);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@NationalNumber", c.NationalNumber);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4, par5);
        }

        public static void UpdateEmployee(Employee c, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Employee SET EmployeeName=@EmployeeName, Address=@Address, Email=@Email, Phone=@Phone, NationalNumber=@NationalNumber WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@EmployeeName", c.EmployeeName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Address", c.Address);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Email", c.Email);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Phone", c.Phone);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@ID", c.ID);
            DbParameter par6 = Database.AddParameter("CashlessAdmin", "@NationalNumber", c.NationalNumber);
            Database.ModifyData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4, par5, par6);
        }

        public static void DeleteEmployee(int id, IEnumerable<Claim> claims)
        {
            // string sql = "DELETE FROM Employee WHERE ID=@ID";
            string sql = "UPDATE Employee SET Hidden=1 WHERE ID=@ID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", id);
            DbConnection con = Database.GetConnection(CreateConnectionString(claims));
            Database.ModifyData(con, sql, par1);
        }
    }
}