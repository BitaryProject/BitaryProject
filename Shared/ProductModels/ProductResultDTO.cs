using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProductModels
{
    public record ProductResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string BrandName { get; set; }
        public string CategoryName { get; set; }
    }
}
