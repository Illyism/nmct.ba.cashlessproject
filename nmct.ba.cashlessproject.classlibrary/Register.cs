using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Register : IFilterableType, IDataErrorInfo
    {
        public int ID { get; set; }
        public string RegisterName { get; set; }
        public string Device { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiresDate { get; set; }

        public Register() {}
        public Register(DbDataReader reader)
        {
            ID = int.Parse(reader["ID"].ToString());
            RegisterName = reader["RegisterName"].ToString();
            Device = reader["Device"].ToString();
            PurchaseDate = DateTime.Parse(reader["PurchaseDate"].ToString());
            ExpiresDate = DateTime.Parse(reader["ExpiresDate"].ToString());
        }


        public string Name { get { return RegisterName; } }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);

                    Validator.ValidateProperty(value, new ValidationContext(this, null, null)
                    {
                        MemberName = columnName
                    });
                }
                catch (ValidationException ex)
                {
                    return ex.Message;
                }
                return String.Empty;
            }
        }

        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }

        public string Error
        {
            get { return "Register"; }
        }
    }

    public class OrganisationRegister
    {
        public int OrganisationID { get; set; }
        public int RegisterID { get; set; }
        public string OrganisationName { get; set; }
        public string RegisterName { get; set; }
        public string Device { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime UntilDate { get; set; }

        public OrganisationRegister() { }
        public OrganisationRegister(DbDataReader reader)
        {
            OrganisationID = int.Parse(reader["OrganisationID"].ToString());
            RegisterID = int.Parse(reader["RegisterID"].ToString());
            OrganisationName = reader["OrganisationName"].ToString();
            RegisterName = reader["RegisterName"].ToString();
            Device = reader["Device"].ToString();
            FromDate = DateTime.Parse(reader["FromDate"].ToString());
            UntilDate = DateTime.Parse(reader["UntilDate"].ToString());
        }
    }

    public class RegisterEmployee
    {
        public int RegisterID { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime UntilTime { get; set; }
        public Employee Employee { get; set; }

        public RegisterEmployee() { }
        public RegisterEmployee(DbDataReader reader)
        {
            RegisterID = int.Parse(reader["RegisterID"].ToString());
            FromTime = DateTime.Parse(reader["FromTime"].ToString());
            UntilTime = DateTime.Parse(reader["UntilTime"].ToString());
            Employee = new Employee
            {
                ID = int.Parse(reader["EmployeeID"].ToString()),
                EmployeeName = reader["EmployeeName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString()
            };
        }

        public bool ShouldSerializeEmployee()
        {
            return (Employee != null);
        }

    }
}
