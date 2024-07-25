using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EkkoSoreeg.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
            private readonly ApplicationDbContext _context;
            public ProductRepository(ApplicationDbContext context) : base(context)
            {
                _context = context;
            }

            public void Update(Product product)
            {
                var productInDb = _context.TbProduct.FirstOrDefault(x => x.Id == product.Id);
            if (productInDb != null)
                {
                productInDb.Name = product.Name;
                productInDb.Description = product.Description;
                productInDb.Price = product.Price;
                productInDb.OfferPrice = product.OfferPrice;
                productInDb.Stock = product.Stock;
                productInDb.CatagoryId = product.CatagoryId;
                productInDb.CreateDate = DateTime.Now;
            }
            }
        }
    }
