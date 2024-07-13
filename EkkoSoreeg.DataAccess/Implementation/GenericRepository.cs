using EkkoSoreeg.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using EkkoSoreeg.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.DataAccess.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? IncludeWord = null)
        {
            IQueryable<T> query = _dbSet;

            if(expression != null)
                query = query.Where(expression);

            if (IncludeWord != null)
            {
                foreach(var item in IncludeWord.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
               
        }

        public T GetFirstorDefault(Expression<Func<T, bool>>? expression = null, string? IncludeWord = null)
        {
            IQueryable<T> query = _dbSet;

            if (expression != null)
                query = query.Where(expression);

            // IncludeWord = logs,perfume,king
            if (IncludeWord != null)
            {
                foreach (var item in IncludeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.SingleOrDefault();
        }

        public void Remove(T entity)
        {
           _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           _dbSet.RemoveRange(entities);
        }
    }
}
