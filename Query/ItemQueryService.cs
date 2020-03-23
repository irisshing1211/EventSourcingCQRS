using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using MongoDB.Driver;

namespace EventSourcingCQRS.Query
{
    public class ItemQueryService : IQueryService<CountItem>
    {
        private readonly QueryContext _ctx;
        public ItemQueryService(QueryContext ctx) { _ctx = ctx; }

        public async Task<List<CountItem>> GetAll() => await _ctx.CountItems.Find(a => true).ToListAsync();
        public CountItem GetById(Guid id) => _ctx.CountItems.Find(a => a.Id == id).FirstOrDefault();
    }
}
