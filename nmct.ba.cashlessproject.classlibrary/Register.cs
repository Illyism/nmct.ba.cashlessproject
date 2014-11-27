using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Register
    {
        public int ID { get; set; }
        public string RegisterName { get; set; }
        public string Device { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiresDate { get; set; }
    }
}
