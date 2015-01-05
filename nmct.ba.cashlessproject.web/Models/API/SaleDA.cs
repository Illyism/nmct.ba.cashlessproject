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
    public class SaleDA
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

        private static Sale BuildModel(DbDataReader reader)
        {
            return new Sale()
            {
                ID = int.Parse(reader["ID"].ToString()),
                Amount = int.Parse(reader["Amount"].ToString()),
                CustomerID = int.Parse(reader["CustomerID"].ToString()),
                RegisterID = int.Parse(reader["RegisterID"].ToString()),
                ProductID = int.Parse(reader["ProductID"].ToString()),
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                TotalPrice = double.Parse(reader["TotalPrice"].ToString()),

                ProductName = reader["ProductName"].ToString(),
                RegisterName = reader["RegisterName"].ToString(),
                CustomerName = reader["CustomerName"].ToString()
            };
        }


        public static List<Sale> GetSales(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "SELECT s.*, c.CustomerName, p.ProductName, r.RegisterName FROM Sale s INNER JOIN Customer c on c.ID=s.CustomerID INNER JOIN Product p ON p.ID=s.ProductID INNER JOIN Register r ON r.ID=s.RegisterID;");
            List<Sale> list = new List<Sale>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static List<Sale> GetSaleByRegister(int id, IEnumerable<Claim> claims)
        {
            string sql = "SELECT s.*, c.CustomerName, p.ProductName, r.RegisterName FROM Sale s INNER JOIN Customer c on c.ID=s.CustomerID INNER JOIN Product p ON p.ID=s.ProductID INNER JOIN Register r ON r.ID=s.RegisterID WHERE s.RegisterID=@RegisterID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterID", id);
            DbDataReader reader = Database.GetData(GetConnection(claims), sql, par1);
            List<Sale> list = new List<Sale>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static List<Sale> GetSaleByProduct(int id, IEnumerable<Claim> claims)
        {
            string sql = "SELECT s.*, c.CustomerName, p.ProductName, r.RegisterName FROM Sale s INNER JOIN Customer c on c.ID=s.CustomerID INNER JOIN Product p ON p.ID=s.ProductID INNER JOIN Register r ON r.ID=s.RegisterID WHERE s.ProductID=@ProductID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ProductID", id);
            DbDataReader reader = Database.GetData(GetConnection(claims), sql, par1);
            List<Sale> list = new List<Sale>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static int InsertSale(Sale s, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Sale VALUES(@Timestamp,@CustomerID,@RegisterID,@ProductID,@Amount,@TotalPrice, 0)";
            DbParameter par0;
            if (s.Timestamp != null)
                par0 = Database.AddParameter("CashlessAdmin", "@Timestamp", s.Timestamp);
            else
                par0 = Database.AddParameter("CashlessAdmin", "@Timestamp", DateTime.Now);
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@CustomerID", s.CustomerID);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@RegisterID", s.RegisterID);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@ProductID", s.ProductID);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@Amount", s.Amount);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@TotalPrice", s.TotalPrice);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par0, par1, par2, par3, par4, par5);
        }
    }
}