using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPRelationshipList : List<SPRelationship>, IEntityList<SPRelationship>
    {
        public SPRelationshipList() : base()
        {
        }

        public SPRelationshipList(int capacity) : base(capacity)
        {
        }

        public SPRelationshipList(IEnumerable<SPRelationship> collection) : base(collection)
        {
        }
    }
}
