using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPRelationshipTypeList : List<SPRelationshipType>, IEntityList<SPRelationshipType>
    {
        public SPRelationshipTypeList() : base()
        {
        }

        public SPRelationshipTypeList(int capacity) : base(capacity)
        {
        }

        public SPRelationshipTypeList(IEnumerable<SPRelationshipType> collection) : base(collection)
        {
        }
    }
}
