﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ProductEntities
{
    public class ProductCategory:BaseEntity<int>
    {
        public string Name { get; set; }

    }
}
