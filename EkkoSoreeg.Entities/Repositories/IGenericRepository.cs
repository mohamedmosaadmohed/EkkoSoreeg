using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // _context.TbCatagory.Where(c => c.Id == Id).ToList();
        // _context.TbCatagory.Include("Perfume").ToList();
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? IncludeWord = null);

        // _context.TbCatagory.Where(c => c.Id == Id).ToList();
        // _context.TbCatagory.Include("Perfume").ToList();
        T GetFirstorDefault(Expression<Func<T, bool>>? expression = null, string? IncludeWord = null);
        // _context.TbCatagory.Add(catagory);
        void Add(T entity);
        // _context.TbCatagory.Remove(item);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
