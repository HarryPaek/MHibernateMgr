using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPPhasePresenter
    {
        SPPhase phase;

        public SPPhasePresenter(SPPhase phase)
        {
            this.phase = phase;
        }

        public override string ToString()
        {
            if (phase == null)
                return "NULL";

            return string.Format("OID=[{0}], ObjectType=[{1}], Name=[{2}], Description=[{3}], CreatedDate=[{4}], CreatedBy=[{5}]",
                                  phase.OId, phase.ObjectType, phase.Name, phase.Description, phase.CreatedDate, phase.CreatedBy.Description);
        }
    }
}
