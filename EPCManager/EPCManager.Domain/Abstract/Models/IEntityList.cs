using EPCManager.Domain.Abstract.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Abstract.Models
{
    public interface IEntityList<TEntity> : IList<TEntity> where TEntity : SPEntity
    {
    }
}
