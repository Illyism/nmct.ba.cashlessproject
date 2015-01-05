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
    public class ProductDA
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

        private static Product BuildModel(DbDataReader reader)
        {
            return new Product()
            {
                ID = int.Parse(reader["ID"].ToString()),
                ProductName = reader["ProductName"].ToString(),
                Price = double.Parse(reader["Price"].ToString())
            };
        }


        public static List<Product> GetProducts(IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Product WHERE Hidden=0;");
            List<Product> list = new List<Product>();
            while (reader.Read()) list.Add(BuildModel(reader));
            reader.Close();
            return list;
        }

        public static Product GetProduct(int id, IEnumerable<Claim> claims)
        {
            DbDataReader reader = Database.GetData(GetConnection(claims), "Select * from Product WHERE ID=@ID AND Hidden=0;");
            reader.Read();
            return BuildModel(reader);
        }

        public static int InsertProduct(Product c, IEnumerable<Claim> claims)
        {
            string sql = "INSERT INTO Product VALUES(@ProductName,@Price, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ProductName", c.ProductName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Price", c.Price);
            return Database.InsertData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2);
        }

        public static void UpdateProduct(Product c, IEnumerable<Claim> claims)
        {
            string sql = "UPDATE Product SET ProductName=@ProductName, Price=@Price WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ProductName", c.ProductName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Price", c.Price);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@ID", c.ID);
            Database.ModifyData(Database.GetConnection(CreateConnectionString(claims)), sql, par1, par2, par3);
        }

        public static void DeleteProduct(int id, IEnumerable<Claim> claims)
        {
            // string sql = "DELETE FROM Product WHERE ID=@ID";
            string sql = "UPDATE Product Hidden=1 WHERE ID=@ID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", id);
            DbConnection con = Database.GetConnection(CreateConnectionString(claims));
            Database.ModifyData(con, sql, par1);
        }
    }
}