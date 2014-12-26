using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Employee : IFilterableType
    {
        public int ID { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public override bool Equals(object obj)
        {
            Employee other = obj as Employee;
            if (other == null) return false;
            return ID.Equals(other.ID);
        }

        public string Name { get { return EmployeeName; } }
    }
}
