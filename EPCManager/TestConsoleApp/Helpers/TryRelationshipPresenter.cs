using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class TryRelationshipPresenter
    {
        TryRelationship relationship;

        public TryRelationshipPresenter(TryRelationship relationship)
        {
            this.relationship = relationship;
        }

        public override string ToString()
        {
            if (relationship == null)
                return "NULL";

            return string.Format("OID=[{0}], LeftObjectType=[{1}], LeftObject=[{2}], RightObjectType=[{3}], RightObject=[{4}], RelationshipType=[{5}], IsReverse=[{6}]",
                relationship.OId, relationship.LeftObjectType, relationship.LeftObject.ToReferenceString(),
                relationship.RightObjectType, relationship.RightObject.ToReferenceString(), relationship.RelType.ToReferenceString(), relationship.IsReverse);
        }
    }
}
