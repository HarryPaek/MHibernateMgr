using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPPhase : SPEntity
    {
        public virtual long OId { get; set; }
        public virtual SPObjectTypes ObjectType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
    }
}
