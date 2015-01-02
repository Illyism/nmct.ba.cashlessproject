using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Employee : IFilterableType, IDataErrorInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is Required")] 
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Address is Required")] 
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is Required")] 
        [EmailAddress(ErrorMessage = "Email Address is Invalid")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is Required")]
        [MaxLength(50,  ErrorMessage = "Phone exceeded 50 letters")]
        public string Phone { get; set; }

        [StringLength(11, MinimumLength=11, ErrorMessage = "National Number must be 11 numbers")]
        public string NationalNumber { get; set; }

        public string Name { get { return EmployeeName; } }

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
            get { return "Employee"; }
        }
    }
}
