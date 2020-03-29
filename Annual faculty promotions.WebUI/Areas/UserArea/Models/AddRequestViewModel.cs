using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class AddRequestViewModel
    {
        [UIHint("PersianDatePicker")]
        public DateTime? LastDateGrade { get; set; }
        public Request Request { get; set; }

        public EducationalResearch EducationalResearch { get; set; }

        public FurtherInformation FurtherInformation { get; set; }
        public AcademicDegree AcademicDegree { get; set; }

        public IEnumerable<UnivercityStructure> UnivercityStructures { get; set; }

        public VeteranType? VeteranType { get; set; }
        public FreedmanOrCaptiveType? FreedmanOrCaptiveType { get; set; }
        public FighterType? FighterType { get; set; }

        public bool Empty
        {
            get { return Request == null ? true : false; }
        }
        public bool IsSent { get; set; }
        public IEnumerable<AttachmentFurtherInformation> AttachmentFurtherInformations { get; set; }

    }
}