using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPDomain : SPEntity, IEquatable<SPDomain>, IComparable<SPDomain>
    {
        public virtual long OId { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual SPPeople CreatedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual SPPeople ModifiedBy { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType() == obj.GetType())
                return Equals(obj as SPDomain);

            return false;
        }

        public override int GetHashCode()
        {
            if (OId > 0) return OId.GetHashCode();

            if (!string.IsNullOrWhiteSpace(Code))
                return Code.GetHashCode();

            return base.GetHashCode();
        }
        
        public virtual bool Equals(SPDomain other)
        {
            if (other == null) return false;

            // OID( > 0 )가 같으면 무조건 동일하다고 간주한다.
            if (OId > 0 && other.OId > 0 && OId == other.OId) return true;

            if (!string.IsNullOrWhiteSpace(Code) && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// 기본 Sort Key는 Code로 한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(SPDomain other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;

            if (string.IsNullOrWhiteSpace(this.Code) && string.IsNullOrWhiteSpace(other.Code)) return 0;
            if (string.IsNullOrWhiteSpace(this.Code)) return -1;
            if (string.IsNullOrWhiteSpace(other.Code)) return 1;

            return this.Code.CompareTo(other.Code);
        }
    }
}
