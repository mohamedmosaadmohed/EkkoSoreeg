using EkkoSoreeg.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Repositories
{
    public interface ISizeRepository : IGenericRepository<ProductSize>
    {
        void Update(ProductSize productSize);
    }
}
