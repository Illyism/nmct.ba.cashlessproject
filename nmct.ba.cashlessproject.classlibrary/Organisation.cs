using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Organisation : IFilterableType, IDataErrorInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Login is Required")] 
        public string Login { get; set; }

        [Required(ErrorMessage = "Password is Required")] 
        public string Password { get; set; }

        [Required(ErrorMessage = "DbName is Required")] 
        public string DbName { get; set; }

        [Required(ErrorMessage = "DbLogin is Required")] 
        public string DbLogin { get; set; }

        [Required(ErrorMessage = "DbPassword is Required")] 
        public string DbPassword { get; set; }

        [Required(ErrorMessage = "Organisation Name is Required")] 
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "Address is Required")] 
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is Required")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is Required")] 
        public string Phone { get; set; }

        public string Name { get { return OrganisationName; } }

        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }

        public string Error
        {
            get { return "Organisation"; }
        }


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
    }
}
