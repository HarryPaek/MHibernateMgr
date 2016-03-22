using EPCManager.Domain.Entities;

namespace EPCManager.Domain.Abstract.Entities
{
    public interface IAssociableEntity
    {
        long OId { get; set; }
        string Revision { get; set; }
        SPClass Class { get; set; }
        string Description { get; set; }
        SPStatus Status { get; set; }
        SPIdentifier Identifier { get; set; }
    }
}
