using EkkoSoreeg.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Repositories
{
    public interface IColorRepository : IGenericRepository<ProductColor>
    {
        void Update(ProductColor productColor);
    }
}
