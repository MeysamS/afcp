using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    // مستندات مطالبه پایه معوق از سال های قبل
    public class AttachmentBasicDelayedPreviousYears : AuditableEntity<int>
    {
        public long FurtherInformationId { get; set; }
        public virtual FurtherInformation FurtherInformation { get; set; }

        public BasicDelayedDocumentType BasicDelayedDocumentType { get; set; }
        public short Year { get; set; }

        public string FileName { get; set; }


    }
}
