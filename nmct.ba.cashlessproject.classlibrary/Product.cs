﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.classlibrary
{
    public class Product : IFilterableType
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

        public string Name { get { return ProductName; } }
    }
}
