using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Customer : IFilterableType, IDataErrorInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 50 characters.")]
        public string Address { get; set; }
        public byte[] Picture { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        [Range(0, 1000, ErrorMessage = "Balance should be between 1 to 1000")] 
        public double Balance { get; set; }

        public string NationalNumber { get; set; }

        public string Name { get { return CustomerName; } }

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
            get { return "Customer"; }
        }
    }
}
