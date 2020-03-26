using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcingCQRS.Query
{
    public interface IQueryService<T>
    {
        Task<List<T>> GetAll();
        T GetById(Guid id);
    }
}
