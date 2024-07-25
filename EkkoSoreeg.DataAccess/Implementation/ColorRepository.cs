using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.DataAccess.Implementation
{
    public class ColorRepository : GenericRepository<ProductColor>,IColorRepository
    {
        private readonly ApplicationDbContext _context;
        public ColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ProductColor productColor)
        {
            var colorInDb = _context.TbProductColors.FirstOrDefault(x => x.Id == productColor.Id);
            if(colorInDb != null)
            {
				colorInDb.Name = productColor.Name;
            }
        }
    }
}
