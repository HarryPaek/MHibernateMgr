using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Abstract.Common
{
    public interface IRelationshipRepository : IRepository<SPRelationship>
    {
        IEnumerable<SPRelationship> GetAll(SPObjectTypes objectType, long entityId);
        IEnumerable<SPRelationship> GetAll(IAssociableEntity entity);
    }
}
