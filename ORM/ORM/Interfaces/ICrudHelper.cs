using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Interfaces
{
    public interface ICrudHelper<T> where T : IBaseModel
    {
        Connection Connection { get; }

        //Async (No sync wrappers)
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync<TResult>(TResult Keys);

        Task<bool> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(T item);

        //Synchronous (No async wrappers)
        IEnumerable<T> GetAll();
        IEnumerable<T> Get<TResult>(TResult Keys);
        bool Insert(T item);
        bool Update(T item);
        bool Delete(T item);
    }
}
