using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPRelationshipType : SPEntity, IEquatable<SPRelationshipType>, IComparable<SPRelationshipType>
    {
        public virtual long OId { get; set; }
        public virtual string Name { get; set; }
        public virtual SPObjectTypes LeftObjectType { get; set; }
        public virtual SPObjectTypes RightObjectType { get; set; }
        public virtual string RelLeftToRight { get; set; }
        public virtual string RelRightToLeft { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual SPPeople ModifiedBy { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType() == obj.GetType())
                return Equals(obj as SPRelationshipType);

            return false;
        }

        public override int GetHashCode()
        {
            if (OId > 0) return OId.GetHashCode();

            if (!string.IsNullOrWhiteSpace(Name)) return Name.GetHashCode();

            if (!string.IsNullOrWhiteSpace(RelLeftToRight) && !string.IsNullOrWhiteSpace(RelRightToLeft))
                return string.Format("{0}|{1}|{2}|{3}", LeftObjectType, RightObjectType, RelLeftToRight, RelRightToLeft).GetHashCode();

            return base.GetHashCode();
        }

        public virtual bool Equals(SPRelationshipType other)
        {
            if (other == null) return false;

            if (OId > 0 && other.OId > 0 && OId == other.OId) return true;

            if (!string.IsNullOrWhiteSpace(Name)) return Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase);

            if (LeftObjectType == other.LeftObjectType && RightObjectType == other.RightObjectType && !string.IsNullOrWhiteSpace(RelLeftToRight) && !string.IsNullOrWhiteSpace(RelRightToLeft))
                return RelLeftToRight.Equals(other.RelLeftToRight, StringComparison.CurrentCultureIgnoreCase) && RelRightToLeft.Equals(other.RelRightToLeft, StringComparison.CurrentCultureIgnoreCase);

            return false;
        }

        /// <summary>
        /// 기본 Sort Key는 Name으로 한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(SPRelationshipType other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;

            if (!string.IsNullOrWhiteSpace(this.Name)) return this.Name.CompareTo(other.Name);
            else return string.IsNullOrWhiteSpace(other.Name) ? 0 : -1;
        }
    }
}
