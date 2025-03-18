using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.BasketEntities
{
    public class BasketItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public int Quantity { get; set; }
    }
}
