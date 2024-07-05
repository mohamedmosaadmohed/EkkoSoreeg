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
    public class CatagoryRepository : GenericRepository<Catagory>,ICatagoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CatagoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Catagory catagory)
        {
            var catagoryInDb = _context.TbCatagory.FirstOrDefault(x => x.Id == catagory.Id);
            if(catagoryInDb != null)
            {
                catagoryInDb.Name = catagory.Name;
                catagoryInDb.Description = catagory.Description;
                catagoryInDb.CreateDate = DateTime.Now;
            }
        }
    }
}
