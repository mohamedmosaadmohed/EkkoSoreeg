using EkkoSoreeg.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.ViewModels
{
    public class ShopViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Catagory> Categories { get; set; }
        public IEnumerable<ProductColor> Colors { get; set; }
        public string SelectedCategory { get; set; }
    }

}
