using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPClassPresenter
    {
        SPClass cls;

        public SPClassPresenter(SPClass cls)
        {
            this.cls = cls;
        }

        public override string ToString()
        {
            return string.Format("OID=[{0}], Name=[{1}], Description=[{2}], ObjectType=[{3}], Parent=[{4}], CreatedDate=[{5}], CreatedBy=[{6}], ModifiedDate=[{7}], ModifiedBy=[{8}]",
                cls.OId, cls.Name, cls.Description, cls.ObjectType, SPEntityExtensions.NullableToString(cls.Parent), cls.CreatedDate,
                cls.CreatedBy.ToReferenceString(), cls.ModifiedDate, SPEntityExtensions.NullableToString(cls.ModifiedBy));
        }
    }
}
