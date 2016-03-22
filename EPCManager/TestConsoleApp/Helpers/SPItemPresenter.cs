using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPItemPresenter
    {
        SPItem item;

        public SPItemPresenter(SPItem item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            if (item == null)
                return "NULL";

            StringBuilder formatText = new StringBuilder();
            formatText.AppendLine("OID=[{0}], Revision=[{1}], RevisionSortSeq=[{2}], Class=[{3}], Description=[{4}], Status=[{5}], Identifier=[{6}], Template=[{7}],");
            formatText.AppendLine("CheckoutDate=[{8}], CheckoutBy=[{9}], CreatedDate=[{10}], CreatedBy=[{11}], ModifiedDate=[{12}], ModifiedBy=[{13}], CompletedDate=[{14}], CompletedBy=[{15}]");
            return string.Format(formatText.ToString(),
                                 item.OId, item.Revision, item.RevisionSortSequence, item.Class.ToReferenceString(), item.Description,
                                 item.Status.ToReferenceString(), item.Identifier.ToReferenceString(), item.Template,
                                 item.CheckoutDate, SPEntityExtensions.NullableToString(item.CheckoutBy), item.CreatedDate, item.CreatedBy.ToReferenceString(),
                                 item.ModifiedDate, SPEntityExtensions.NullableToString(item.ModifiedBy), item.CompletedDate, SPEntityExtensions.NullableToString(item.CompletedBy));
        }
    }
}
