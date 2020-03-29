using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IRequestService
    {
        void AddNewRequest(Request request);
        IList<Request> GetAllRequests();
        IQueryable<Request> GetAllRequestsAsQueryable();
        bool Delete(long id);
        Request Find(long? id);
        EducationalResearch FindEducationResearch(int? id);
        ScientificExecutive FindScientificExecutive(int? id);
        Dissertation FindDissertation(long? id);
        Technology FindTechnology(long? id);
        FurtherInformation FindFurtherInformation(long? id);
        IQueryable<Request> Where(Expression<Func<Request, bool>> predicate);
        IQueryable<FurtherInformation> Where(Expression<Func<FurtherInformation, bool>> predicate);
        void EditRequest(Request request);
        void EditFurtherInformation(FurtherInformation furtherInformation);
        void EditEducationalResearch(EducationalResearch educationalResearch);
        void EditDissertation(Dissertation dissertation);
        void EditScientificExecutive(ScientificExecutive scientificExecutive);
        void EditTechnology(Technology technology);
        void AddOrUpdateTechnologyDetail(TechnologyDetail technologyDetail);
        IQueryable<EducationalResearch> GetEducation_ByRequestId_ForCurrentYear(long requestId);
        IQueryable<EducationalResearch> GetReasearch_ByRequestId_ForCurrentYear(long requestId);
        IQueryable<ScientificExecutive> GetScientificExecutive_ByRequestId_ForCurrentYear(long requestId);
        IQueryable<Technology> GetTechnology_ByRequestId_ForCurrentYear(long requestId);
        IQueryable<Dissertation> GetDissertation_ByRequestId_ForCurrentYear(long requestId);
        //IQueryable<EducationalResearch> Where(Expression<Func<EducationalResearch, bool>> predicate);
        //IQueryable<ScientificExecutive> Where(Expression<Func<ScientificExecutive, bool>> predicate);
        IQueryable<EducationalResearch> WhereEducationalResearch(Expression<Func<EducationalResearch, bool>> predicate);
        IQueryable<Dissertation> WhereDissertation(Expression<Func<Dissertation, bool>> predicate);
        IQueryable<Technology> WhereTechnology(Expression<Func<Technology, bool>> predicate);
        IQueryable<TechnologyDetail> WhereTechnologyDetail(Expression<Func<TechnologyDetail, bool>> predicate);
        IQueryable<ScientificExecutive> WhereScientificExecutive(Expression<Func<ScientificExecutive, bool>> predicate);

        //IQueryable<Dissertation> Where(Expression<Func<Dissertation, bool>> predicate); 
        void AddDissertation(Dissertation dissertation);
        void AddEducationReasearch(EducationalResearch educationalResearch);
        void AddScientificExecutive(ScientificExecutive scientificExecutive);
        void AddTechnology(Technology technology);
        void AddFurtherInformation(FurtherInformation furtherInformation);
        bool DeleteEducationReasearch(int id);
        bool DeleteReasearchAttach(long id);
        bool DeleteScientificExecutive(int id);
        bool DeleteDissertation(long id);
        bool DeleteTechnology(long id);
        bool DeleteTechnologyDetail(long TechnologyId, long RequestId);
        bool DeleteListofTechnologyDetail(long RequestId);
        void AddAttachmentResearch(AttachmentResearch attachment);
        void AddAttachmentFurtherInformation(AttachmentFurtherInformation attachmentFurther);
        IQueryable<AttachmentResearch> GetAttachmentResearches(long reaserchId);
        AttachmentResearch FindAttachforResearch(long reaserchAttachId);
        IQueryable<AttachmentFurtherInformation> GetAttachmentFurtherInformations(long furtherInformationId, int furtherInformationType);
        IQueryable<AttachmentFurtherInformation> GetAttachmentFurtherInformations(long furtherInformationId);
    }
}
