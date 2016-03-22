using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPStatusPresenter
    {
        SPStatus status;

        public SPStatusPresenter(SPStatus status)
        {
            this.status = status;
        }

        public override string ToString()
        {
            if (status == null)
                return "NULL";

            return string.Format("OID=[{0}], ObjectType=[{1}], Name=[{2}], Description=[{3}], Ordinal=[{4}]",
                                  status.OId, status.ObjectType, status.Name, status.Description, status.Ordinal);
        }
    }
}
