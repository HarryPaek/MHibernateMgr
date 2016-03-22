using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPIdentifier : SPEntity, IEquatable<SPIdentifier>, IComparable<SPIdentifier>
    {
        public virtual long OId { get; set; }
        public virtual SPDomain Domain { get; set; }
        public virtual string Code { get; set; }
        public virtual SPObjectTypes ObjectType { get; set; }

        public virtual SPIdentifier Clone()
        {
            return new SPIdentifier
            {
                OId = this.OId,
                Domain = this.Domain,
                Code = this.Code,
                ObjectType = this.ObjectType
            };
        }

        public override bool Equals(object obj)
        {
            if (obj != null && GetType() == obj.GetType())
                return Equals(obj as SPIdentifier);

            return false;
        }

        public override int GetHashCode()
        {
            if (OId > 0) return OId.GetHashCode();

            if (Domain != null && !string.IsNullOrWhiteSpace(Domain.Code) && !string.IsNullOrWhiteSpace(Code))
                return string.Format("{0}|{1}|{2}", ObjectType, Domain.Code, Code).GetHashCode();

            return base.GetHashCode();
        }

        public virtual bool Equals(SPIdentifier other)
        {
            if (other == null) return false;

            // OID( > 0 )가 같으면 무조건 동일하다고 간주한다.
            if (OId > 0 && other.OId > 0 && OId == other.OId) return true;

            // ObjectType이 같고, Domain이 같은 경우에 다음으로 진행
            if (ObjectType == other.ObjectType && Domain != null && Domain.Equals(other.Domain))
            {
                //Code가 같으면 동일하다
                if (!string.IsNullOrWhiteSpace(Code) && Code.Equals(other.Code, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        /// <summary>
        /// 기본 Sort Key는 ObjectType, Domain.Code, Code로 한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(SPIdentifier other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;

            int compareResult = this.ObjectType.CompareTo(other.ObjectType);

            if (compareResult == 0)
            {
                if (this.Domain != null)
                    compareResult = this.Domain.CompareTo(other.Domain);
                else compareResult = other.Domain == null ? 0 : -1;
            }

            if (compareResult == 0)
            {
                if (!string.IsNullOrWhiteSpace(this.Code))
                    compareResult = this.Code.CompareTo(other.Code);
                else compareResult = string.IsNullOrWhiteSpace(other.Code) ? 0 : -1;
            }

            return compareResult;
        }
    }
}
