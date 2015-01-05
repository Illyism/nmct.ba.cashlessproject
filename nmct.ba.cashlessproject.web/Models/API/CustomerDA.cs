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
    public class CustomerDA
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

        private static Customer BuildModel(DbDataReader reader)
        {
            return new Customer()
            {
                ID = int.Parse(reader["ID"].ToString()),
                CustomerName = reader["CustomerName"].ToString(),
                Address = reader["Address"].ToString(),
                Balance = double.Parse(reader["Balance"].ToString()),
                NationalNumber = reader["NationalNumber"].ToString()
            };
        }


        public static List<Customer> GetCustomers(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Customer WHERE Hidden=0;");
            List<Customer> list = new List<Customer>();
            if (reader.HasRows == false)
            {
                reader.Close();
                return null;
            }
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static Customer GetCustomer(int id, IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Customer WHERE ID=@ID AND Hidden=0");
            reader.Read();
            return BuildModel(reader);
        }

        public static Customer GetCustomerByNationalNumber(string id, IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Customer WHERE NationalNumber=@NationalNumber AND Hidden=0",
                Database.AddParameter("CashlessAdmin", "@NationalNumber", id));
            reader.Read();
            if (reader.HasRows == false) return null;
            return BuildModel(reader);
        }


        public static int InsertCustomer(Customer c, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Customer VALUES(@CustomerName,@Address,@Picture,@Balance,@NationalNumber, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@CustomerName", c.CustomerName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Address", c.Address);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Picture", c.Picture ?? new byte[0]);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Balance", c.Balance);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@NationalNumber", c.NationalNumber);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4,par5);
        }

        public static void UpdateCustomer(Customer c, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Customer SET CustomerName=@CustomerName, Address=@Address, Picture=@Picture, Balance=@Balance WHERE NationalNumber=@NationalNumber AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@CustomerName", c.CustomerName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Address", c.Address);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Picture", c.Picture ?? new byte[0]);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Balance", c.Balance);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@NationalNumber", c.NationalNumber);
            Database.ModifyData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3, par4, par5);
        }

        public static void DeleteCustomer(int id, IEnumerable<Claim> claims)
        {
            // string sql = "DELETE FROM Customer WHERE ID=@ID";
            string sql = "UPDATE Customer SET Hidden=1 WHERE ID=@ID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", id);
            DbConnection con = Database.GetConnection(CreateConnectionString(claims));
            Database.ModifyData(con, sql, par1);
        }

        public static void AddSale(Sale s, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Customer SET Balance=Balance-@TotalPrice WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@TotalPrice", s.TotalPrice);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@ID", s.CustomerID);
            Database.ModifyData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2);
        }
    }
}