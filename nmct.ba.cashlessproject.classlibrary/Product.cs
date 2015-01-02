using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Product : IFilterableType, IDataErrorInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")] 
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required")] 
        public double Price { get; set; }

        public string Name { get { return ProductName; } }

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
            get { return "Product"; }
        }
    }
}
