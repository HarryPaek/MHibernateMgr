using EPCManager.Domain.Abstract.Entities;
using System;
using System.Collections.Generic;

namespace EPCManager.Domain.Entities
{
    public class SPItem : SPEntity, ILockableEntity, ILifeCycleEntity, ICopiableEntity, IAssociableEntity, IEquatable<SPItem>, IComparable<SPItem>
    {
        private IList<SPRelationship> _relationships;

        public SPItem()
        {
            _relationships = new List<SPRelationship>();
        }

        public virtual long OId { get; set; }
        public virtual string Revision{ get; set; }
        public virtual int RevisionSortSequence { get; set; }
        public virtual SPClass Class { get; set; }
        public virtual string Description { get; set; }
        public virtual SPStatus Status { get; set; }
        public virtual SPIdentifier Identifier { get; set; }
        public virtual bool Template { get; set; }
        public virtual DateTime? CheckoutDate { get; set; }
        public virtual SPPeople CheckoutBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual SPPeople ModifiedBy { get; set; }
        public virtual DateTime? CompletedDate { get; set; }
        public virtual SPPeople CompletedBy { get; set; }

        public virtual IList<SPRelationship> Relationships
        {
            get { return _relationships; }
        }

        /**
         * Item이 지정된 Status에 있는지 여부 
         **/
        public virtual bool IsStatusAt(SPStatus status)
        {
            return this.Status.Equals(status);
        }

        public virtual SPItem Clone()
        {
            return new SPItem
            {
                OId = this.OId,
                Revision = this.Revision,
                RevisionSortSequence = this.RevisionSortSequence,
                Class = this.Class,
                Description = this.Description,
                Status = this.Status,
                Identifier = this.Identifier.Clone(),
                Template = this.Template,
                CheckoutDate = this.CheckoutDate,
                CheckoutBy = this.CheckoutBy,
                CreatedDate = this.CreatedDate,
                CreatedBy = this.CreatedBy,
                ModifiedDate = this.ModifiedDate,
                ModifiedBy = this.ModifiedBy,
                CompletedDate = this.CompletedDate,
                CompletedBy = this.CompletedBy
            };
        }

        ICopiableEntity ICopiableEntity.Clone()
        {
            return Clone();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType() == obj.GetType())
                return Equals(obj as SPItem);

            return false;
        }

        public override int GetHashCode()
        {
            if (OId > 0) return OId.GetHashCode();

            if (Identifier != null && !string.IsNullOrWhiteSpace(Revision))
                return string.Format("{0}|{1}", Identifier.GetHashCode(), Revision).GetHashCode();

            return base.GetHashCode();
        }

        public virtual bool Equals(SPItem other)
        {
            if (other == null) return false;

            // OID( > 0 )가 같으면 무조건 동일하다고 간주한다.
            if (OId > 0 && other.OId > 0 && OId == other.OId) return true;

            // Identiifer가 같은 경우에 다음으로 진행
            if (Identifier != null && Identifier.Equals(other.Identifier))
            {
                //Revision이 같으면 동일하다
                if (!string.IsNullOrWhiteSpace(Revision) && Revision.Equals(other.Revision, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        /// <summary>
        /// 기본 Sort Key는 Identifier, RevisionSortSequence로 한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(SPItem other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;

            int compareResult = 0;

            if (this.Identifier != null)
                compareResult = this.Identifier.CompareTo(other.Identifier);
            else compareResult = other.Identifier == null ? 0 : -1;

            if (compareResult == 0)
                compareResult = this.RevisionSortSequence.CompareTo(other.RevisionSortSequence);

            return compareResult;
        }
    }
}
