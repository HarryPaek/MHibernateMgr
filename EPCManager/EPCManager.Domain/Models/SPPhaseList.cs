using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPPhaseList : List<SPPhase>, IEntityList<SPPhase>
    {
        public SPPhaseList() : base()
        {
        }

        public SPPhaseList(int capacity) : base(capacity)
        {
        }

        public SPPhaseList(IEnumerable<SPPhase> collection) : base(collection)
        {
        }
    }
}
