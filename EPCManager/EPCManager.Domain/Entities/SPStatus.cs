using EPCManager.Domain.Abstract.Entities;
using System;

namespace EPCManager.Domain.Entities
{
    public class SPStatus : SPEntity, IEquatable<SPStatus>, IComparable<SPStatus>
    {
        public virtual long OId { get; set; }
        public virtual SPObjectTypes ObjectType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Ordinal { get; set; }

        public override bool Equals(object obj)
        {
            if(obj != null && GetType() == obj.GetType())
                return Equals(obj as SPStatus);

            return false;
        }

        public override int GetHashCode()
        {
            if (OId > 0) return OId.GetHashCode();

            if (!string.IsNullOrWhiteSpace(Name))
                return string.Format("{0}|{1}", ObjectType, Name).GetHashCode();

            return base.GetHashCode();
        }

        public virtual bool Equals(SPStatus other)
        {
            if (other == null) return false;

            if (OId > 0 && other.OId > 0 && OId == other.OId) return true;

            if (ObjectType == other.ObjectType && !string.IsNullOrWhiteSpace(Name) && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// 기본 Sort Key는 ObjectType, Name으로 한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(SPStatus other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;

            int compareResult = this.ObjectType.CompareTo(other.ObjectType);

            if (compareResult == 0)
            {
                if (!string.IsNullOrWhiteSpace(this.Name))
                    compareResult = this.Name.CompareTo(other.Name);
                else compareResult = string.IsNullOrWhiteSpace(other.Name) ? 0 : -1;
            }

            return compareResult;
        }
    }
}
