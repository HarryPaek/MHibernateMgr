using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal static class SPEntityExtensions
    {
        public static string NullableToString(SPPeople people)
        {
            return people != null ? people.ToReferenceString() : string.Empty;
        }

        public static string ToReferenceString(this SPPeople people)
        {
            return people.Description;
        }

        public static string NullableToString(SPClass cls)
        {
            return cls != null ? cls.ToReferenceString() : string.Empty;
        }

        public static string ToReferenceString(this SPClass cls)
        {
            return string.Format("{0}-{1}", cls.Name, cls.Description);
        }

        public static string ToReferenceString(this SPPhase phase)
        {
            return string.Format("{0}-{1}", phase.Name, phase.Description);
        }

        public static string ToReferenceString(this SPStatus status)
        {
            return string.IsNullOrWhiteSpace(status.Description) ? status.Name : status.Description;
        }

        public static string ToReferenceString(this SPDomain domain)
        {
            return domain.Code;
        }

        public static string ToReferenceString(this SPDocument document)
        {
            return string.Format("{0},{1}", document.OId, document.Identifier.ToReferenceString());
        }

        public static string ToReferenceString(this SPIdentifier identifier)
        {
            return string.Format("{0}|{1}", identifier.Domain.ToReferenceString(), identifier.Code);
        }

        public static string ToReferenceString(this SPRelationshipType relType)
        {
            if (relType == null)
                return "NULL";

            return string.Format("{0}|{1}[{2}]<->{3}[{4}]", relType.Name, relType.RelLeftToRight, relType.LeftObjectType, relType.RelRightToLeft, relType.RightObjectType);
        }

        public static string ToReferenceString(this IAssociableEntity associableEntity)
        {
            if (associableEntity == null)
                return "NULL";

            return string.Format("{0},{1}", associableEntity.OId, associableEntity.Identifier.ToReferenceString());
        }
    }
}
