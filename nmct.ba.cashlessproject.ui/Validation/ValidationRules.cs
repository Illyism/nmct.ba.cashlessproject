using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace nmct.ba.cashlessproject.ui.Validation
{
    class NumberBetweenRule : ValidationRule
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Key { get; set; }

        public override ValidationResult Validate(object value,
        System.Globalization.CultureInfo cultureInfo)
        {
            int i;
            if (Int32.TryParse((string)value, out i))
            {
                if (i < Minimum)
                    return new ValidationResult(false, Key + " must be between " + Minimum + " and " + Maximum);
                else if (i > Maximum)
                    return new ValidationResult(false, Key + " must be between " + Minimum + " and " + Maximum);
                else if (i >= Minimum && i <= Maximum)
                    return new ValidationResult(true, null);
                else return new ValidationResult(false, Key + " is not valid.");
            }
            else
                return new ValidationResult(false, Key + " is not a number");
        }
    }
}
