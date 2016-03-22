using EPCManager.Domain.Abstract.Entities;

namespace EPCManager.Domain.Entities
{
    public class AssociableEntity : IAssociableEntity
    {
        public virtual long OId { get; set; }
        public virtual string Revision { get; set; }
        public virtual SPClass Class { get; set; }
        public virtual string Description { get; set; }
        public virtual SPStatus Status { get; set; }
        public virtual SPIdentifier Identifier { get; set; }
    }
}
