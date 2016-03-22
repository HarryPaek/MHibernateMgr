using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPClassList : List<SPClass>, IEntityList<SPClass>
    {
        public SPClassList() : base()
        {
        }

        public SPClassList(int capacity) : base(capacity)
        {
        }

        public SPClassList(IEnumerable<SPClass> collection) : base(collection)
        {
        }
    }
}
