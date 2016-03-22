using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPRelationshipTypePresenter
    {
        SPRelationshipType relType;

        public SPRelationshipTypePresenter(SPRelationshipType relType)
        {
            this.relType = relType;
        }

        public override string ToString()
        {
            if (relType == null)
                return "NULL";

            return string.Format("OID=[{0}], Name=[{1}], LeftObjectType=[{2}], RightObjectType=[{3}], RelLeftToRight=[{4}], RelRightToLeft=[{5}], IsSystem=[{6}], Description=[{7}]",
                relType.OId, relType.Name, relType.LeftObjectType, relType.RightObjectType, relType.RelLeftToRight, relType.RelRightToLeft, relType.IsSystem, relType.Description);
        }
    }
}
