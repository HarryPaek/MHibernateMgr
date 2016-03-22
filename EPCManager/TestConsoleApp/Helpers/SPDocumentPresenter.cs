using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPDocumentPresenter
    {
        SPDocument document;

        public SPDocumentPresenter(SPDocument document)
        {
            this.document = document;
        }

        public override string ToString()
        {
            StringBuilder formatText = new StringBuilder();
            formatText.AppendLine("OID=[{0}], Revision=[{1}], RevisionSortSeq=[{2}], Class=[{3}], Description=[{4}], Status=[{5}], Phase=[{6}], Identifier=[{7}], Template=[{8}],");
            formatText.AppendLine("CheckoutDate=[{9}], CheckoutBy=[{10}], CreatedDate=[{11}], CreatedBy=[{12}], ModifiedDate=[{13}], ModifiedBy=[{14}], CompletedDate=[{15}], CompletedBy=[{16}]");
            return string.Format(formatText.ToString(),
                                 document.OId, document.Revision, document.RevisionSortSequence, document.Class.ToReferenceString(), document.Description,
                                 document.Status.ToReferenceString(), document.Phase.ToReferenceString(), document.Identifier.ToReferenceString(), document.Template,
                                 document.CheckoutDate, SPEntityExtensions.NullableToString(document.CheckoutBy), document.CreatedDate, document.CreatedBy.ToReferenceString(),
                                 document.ModifiedDate, SPEntityExtensions.NullableToString(document.ModifiedBy), document.CompletedDate, SPEntityExtensions.NullableToString(document.CompletedBy));
        }
    }
}
