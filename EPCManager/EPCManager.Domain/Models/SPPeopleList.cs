using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPPeopleList : List<SPPeople>, IEntityList<SPPeople>
    {
        public SPPeopleList() : base()
        {
        }

        public SPPeopleList(int capacity) : base(capacity)
        {
        }

        public SPPeopleList(IEnumerable<SPPeople> collection) : base(collection)
        {
        }
    }
}
