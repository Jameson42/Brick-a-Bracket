using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrickABracket.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core.ORM
{
    public class Repository<T> : IDisposable where T : class, IDBItem
    {
        internal readonly DbContext _context;
        internal readonly DbSet<T> _set;
        public Repository(DbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }
        public int Create(T t)
        {
            _set.Add(t);
            _context.SaveChanges();
            return t._id;
        }
        public async Task<int> CreateAsync(T t)
        {
            _set.Add(t);
            await _context.SaveChangesAsync();
            return t._id;
        }
        public virtual T Read(int id) => _set.Find(id);
        public virtual async Task<T> ReadAsync(int id) => await _set.FindAsync(id);
        public virtual IEnumerable<T> Read(IEnumerable<int> ids) =>
            _set.Where(t => ids.Contains(t._id));
        public virtual IAsyncEnumerable<T> ReadAsync(IEnumerable<int> ids) =>
            _set.Where(t => ids.Contains(t._id)).AsAsyncEnumerable();
        public virtual IEnumerable<T> ReadAll() => _set.AsEnumerable();
        public virtual IAsyncEnumerable<T> ReadAllAsync() => _set.AsAsyncEnumerable();
        public bool Update(T t)
        {
            try
            {
                _set.Update(t);
                _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateAsync(T t)
        {
            try
            {
                _set.Update(t);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public bool Delete(T t)
        {
            _set.Remove(t);
            _context.SaveChanges();
            return true;
        }
        public bool Delete(int id) => Delete(Read(id));
        public async Task<bool> DeleteAsync(T t)
        {
            _set.Remove(t);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id) => await DeleteAsync(await ReadAsync(id));

        public void Dispose() => _context.Dispose();
    }
}