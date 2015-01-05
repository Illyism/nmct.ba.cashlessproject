using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace nmct.ba.cashlessproject.web.Models
{
    public class RegisterDA
    {

        public static List<Register> GetRegisters()
        {
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from Register WHERE Hidden=0");
            List<Register> list = new List<Register>();
            while (reader.Read()) list.Add(new Register(reader));
            reader.Close();
            return list;
        }

        public static Register GetRegister(int id)
        {
            DbDataReader reader = Database.GetData("CashlessAdmin", "Select * from Register WHERE ID=@ID AND Hidden=0;",
                Database.AddParameter("CashlessAdmin", "@ID", id));
            reader.Read();
            return new Register(reader);
        }

        public static int InsertRegister(Register model)
        {
            string sql = "INSERT INTO Register VALUES(@RegisterName,@Device,@PurchaseDate,@ExpiresDate, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterName", model.RegisterName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Device", model.Device);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@PurchaseDate", model.PurchaseDate);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@ExpiresDate", model.ExpiresDate);
            return Database.InsertData("CashlessAdmin", sql, par1, par2, par3, par4);
        }

        public static void UpdateRegister(Register model)
        {
            string sql = "UPDATE Register SET RegisterName=@RegisterName, Device=@Device, PurchaseDate=@PurchaseDate, ExpiresDate=@ExpiresDate WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterName", model.RegisterName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Device", model.Device);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@PurchaseDate", model.PurchaseDate);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@ExpiresDate", model.ExpiresDate);
            DbParameter par5 = Database.AddParameter("CashlessAdmin", "@ID", model.ID);
            Database.ModifyData("CashlessAdmin", sql, par1, par2, par3, par4, par5);

            UpdateRegisterOrganisation(model);
        }

        

        public static void DeleteRegister(int id)
        {
            UnAssignRegister(id);
            // string sql = "DELETE FROM Register WHERE ID=@ID";
            string sql = "UPDATE Register SET Hidden=1 WHERE ID=@ID";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", id);
            Database.ModifyData("CashlessAdmin", sql, par1);
        }



        public static List<OrganisationRegister> GetOrganisationRegisters()
        {
            string sql = "SELECT o.OrganisationName, r.RegisterName, r.Device, orr.* FROM Organisation_Register orr INNER JOIN Register r ON r.ID=orr.RegisterID INNER JOIN Organisations o ON o.ID=orr.OrganisationID WHERE orr.Hidden=0 AND o.Hidden=0 AND r.Hidden=0;";
            DbDataReader reader = Database.GetData("CashlessAdmin", sql);
            List<OrganisationRegister> list = new List<OrganisationRegister>();
            while (reader.Read()) list.Add(new OrganisationRegister(reader));
            return list;
        }

        public static List<Register> GetUnusedRegisters()
        {
            string sql = "SELECT r.* FROM Register r WHERE r.ID NOT IN (SELECT orr.RegisterID FROM Organisation_Register orr WHERE Hidden=0) AND r.Hidden=0";
            List<Register> list = new List<Register>();
            DbDataReader reader = Database.GetData("CashlessAdmin", sql);
            while (reader.Read()) list.Add(new Register(reader));
            reader.Close();
            return list;
        }

        public static OrganisationRegister GetOrganisationRegister(int regId)
        {
            string sql = "SELECT o.OrganisationName, r.RegisterName, r.Device, orr.* FROM Organisation_Register orr INNER JOIN Register r ON r.ID=orr.RegisterID INNER JOIN Organisations o ON o.ID=orr.OrganisationID WHERE orr.RegisterID=@regId AND orr.Hidden=0 AND o.HIDDEN=0 AND r.HIDDEN=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@regId", regId);
            DbDataReader reader = Database.GetData("CashlessAdmin", sql, par1);
            reader.Read();
            return new OrganisationRegister(reader);
        }

        public static void AssignRegister(OrganisationRegister model)
        {
            string sql = "INSERT INTO Organisation_Register VALUES(@OrganisationID,@RegisterID,@FromDate,@UntilDate, 0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@OrganisationID", model.OrganisationID);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@RegisterID", model.RegisterID);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@FromDate", model.FromDate);
            DbParameter par4 = Database.AddParameter("CashlessAdmin", "@UntilDate", model.UntilDate);
            Database.InsertData("CashlessAdmin", sql, par1, par2, par3, par4);

            AssignRegister(model, model.OrganisationID);
        }

        private static void AssignRegister(OrganisationRegister model, int orgId)
        {
            Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(orgId));
            ConnectionStringSettings settings = Database.CreateConnectionString(org.DbName);

            string sql = "INSERT INTO Register (ID, RegisterName, Device, Hidden) VALUES(@ID,@RegisterName,@Device,0)";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@ID", model.RegisterID);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@RegisterName", model.RegisterName);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@Device", model.Device);
            Database.InsertData(Database.GetConnection(settings), sql, par1, par2, par3);
        }

        public static int UnAssignRegister(int regId)
        {
            // string sql = "DELETE FROM Organisation_Register WHERE RegisterID=@regId";
            string sql = "UPDATE Organisation_Register SET Hidden=1 WHERE RegisterID=@regId";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@regId", regId);
            return Database.ModifyData("CashlessAdmin", sql, par1);
        }

        private static void UpdateRegisterOrganisation(Register model)
        {
            OrganisationRegister orgReg = GetOrganisationRegister(model.ID);
            Organisation org = OrganisationDA.DecryptOrganisation(OrganisationDA.GetOrganisation(orgReg.OrganisationID));
            ConnectionStringSettings settings = Database.CreateConnectionString(org.DbName);

            string sql = "UPDATE Register SET RegisterName=@RegisterName, Device=@Device WHERE ID=@ID AND Hidden=0";
            DbParameter par1 = Database.AddParameter("CashlessAdmin", "@RegisterName", model.RegisterName);
            DbParameter par2 = Database.AddParameter("CashlessAdmin", "@Device", model.Device);
            DbParameter par3 = Database.AddParameter("CashlessAdmin", "@ID", model.ID);
            Database.ModifyData(Database.GetConnection(settings), sql, par1, par2, par3);
        }
    }
}