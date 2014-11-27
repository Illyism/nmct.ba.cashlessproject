using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class ErrorLog
    {
        public int RegisterID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Stacktrace { get; set; }
    }
}
