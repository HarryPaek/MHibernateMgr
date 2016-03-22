using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Abstract.Models;
using System.Collections.Generic;

namespace EPCManager.Domain.Abstract.Common
{
    public interface IRepository<TEntity> where TEntity : SPEntity
    {
        TEntity Get(long id);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
        IEnumerable<TEntity> Items { get; }

        IEntityList<TEntity> GetAllAsList();
    }
}
