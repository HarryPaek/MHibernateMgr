using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Abstract.Common
{
    public interface ITryRelationshipRepository : IRepository<TryRelationship>
    {
        IEnumerable<TryRelationship> GetAll(SPObjectTypes objectType, long entityId);
        IEnumerable<TryRelationship> GetAll(IAssociableEntity entity);
    }
}
