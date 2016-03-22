using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPPeople : SPEntity
    {
        public virtual long OId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Description { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual char Inactive { get; set; }
        public virtual SPIdentifier Identifier { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual SPPeople ModifiedBy { get; set; }
    }
}
