using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;


namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfRequestService : IRequestService
    {
        IUnitOfWork _uow;
        readonly IDbSet<Request> _requests;
        readonly IDbSet<EducationalResearch> _educationalResearches;
        readonly IDbSet<ScientificExecutive> _scientificExecutives;
        readonly IDbSet<AttachmentResearch> _attachmentResearches;
        readonly IDbSet<Dissertation> _dissertations;
        readonly IDbSet<Technology> _technologies;
        readonly IDbSet<TechnologyDetail> _technologyDetails;
        readonly IDbSet<FurtherInformation> _furtherInformations;
        readonly IDbSet<AttachmentFurtherInformation> _attachmentFurtherInformations;
        public EfRequestService(IUnitOfWork uow)
        {
            _uow = uow;
            _requests = _uow.Set<Request>();
            _educationalResearches = _uow.Set<EducationalResearch>();
            _scientificExecutives = _uow.Set<ScientificExecutive>();
            _attachmentResearches = _uow.Set<AttachmentResearch>();
            _dissertations = _uow.Set<Dissertation>();
            _technologies = _uow.Set<Technology>();
            _technologyDetails = _uow.Set<TechnologyDetail>();
            _furtherInformations = _uow.Set<FurtherInformation>();
            _attachmentFurtherInformations = _uow.Set<AttachmentFurtherInformation>();
        }

        public void AddNewRequest(Request request)
        {
            _requests.Add(request);
        }

        public void AddDissertation(Dissertation dissertation)
        {
            _dissertations.Add(dissertation);
        }

        public void AddEducationReasearch(EducationalResearch educationalResearch)
        {
            _educationalResearches.Add(educationalResearch);
        }


        public void AddScientificExecutive(ScientificExecutive scientificExecutive)
        {
            _scientificExecutives.Add(scientificExecutive);
        }

        public void AddTechnology(Technology technology)
        {
            _technologies.Add(technology);
        }

        public void AddFurtherInformation(FurtherInformation furtherInformation)
        {
            _furtherInformations.Add(furtherInformation);
        }

        public bool DeleteEducationReasearch(int id)
        {
            try
            {
                var entity = _educationalResearches.Find(id);
                _educationalResearches.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public bool DeleteReasearchAttach(long id)
        {
            try
            {
                var entity = _attachmentResearches.Find(id);
                _attachmentResearches.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteScientificExecutive(int id)
        {
            try
            {
                var entity = _scientificExecutives.Find(id);
                _scientificExecutives.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteDissertation(long id)
        {
            try
            {
                var entity = _dissertations.Find(id);
                _dissertations.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool DeleteTechnology(long id)
        {
            try
            {
                var entity = _technologies.Find(id);
                _technologies.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTechnologyDetail(long TechnologyId, long RequestId)
        {
            try
            {
                var entity = _technologyDetails.Where(t => t.TechnologyId == TechnologyId && t.RequestId == RequestId).FirstOrDefault();
                if (entity != null)
                    _technologyDetails.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteListofTechnologyDetail(long RequestId)
        {
            try
            {
                var listTechnologyDetail = _technologyDetails.Where(t => t.RequestId == RequestId).ToList();
                foreach (var item in listTechnologyDetail)
                {
                    _technologyDetails.Remove(item);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void AddAttachmentResearch(AttachmentResearch attachment)
        {
            _attachmentResearches.Add(attachment);
        }

        public void AddAttachmentFurtherInformation(AttachmentFurtherInformation attachmentFurther)
        {
            _attachmentFurtherInformations.Add(attachmentFurther);
        }


        public IList<Request> GetAllRequests()
        {
            return _requests.ToList();
        }

        public IQueryable<Request> GetAllRequestsAsQueryable()
        {
            return _requests.AsQueryable();
        }

        public bool Delete(long id)
        {
            try
            {
                var entity = _requests.Find(id);
                _requests.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public Request Find(long? id)
        {
            return _requests.Find(id);
        }

        public EducationalResearch FindEducationResearch(int? id)
        {
            return _educationalResearches.Find(id);
        }
        public ScientificExecutive FindScientificExecutive(int? id)
        {
            return _scientificExecutives.Find(id);
        }

        public Dissertation FindDissertation(long? id)
        {
            return _dissertations.Find(id);
        }

        public Technology FindTechnology(long? id)
        {
            return _technologies.Find(id);
        }

        public FurtherInformation FindFurtherInformation(long? id)
        {
            return _furtherInformations.Find(id);
        }

        public IQueryable<Request> Where(Expression<Func<Request, bool>> predicate)
        {
            try
            {
                return _requests.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<FurtherInformation> Where(Expression<Func<FurtherInformation, bool>> predicate)
        {
            try
            {
                return _furtherInformations.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<EducationalResearch> WhereEducationalResearch(Expression<Func<EducationalResearch, bool>> predicate)
        {
            try
            {
                return _educationalResearches.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<Dissertation> WhereDissertation(Expression<Func<Dissertation, bool>> predicate)
        {
            try
            {
                return _dissertations.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<Technology> WhereTechnology(Expression<Func<Technology, bool>> predicate)
        {
            try
            {
                return _technologies.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<TechnologyDetail> WhereTechnologyDetail(Expression<Func<TechnologyDetail, bool>> predicate)
        {
            try
            {
                return _technologyDetails.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<ScientificExecutive> WhereScientificExecutive(Expression<Func<ScientificExecutive, bool>> predicate)
        {
            try
            {
                return _scientificExecutives.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public IQueryable<EducationalResearch> GetEducation_ByRequestId_ForCurrentYear(long requestId)
        {
            return _educationalResearches.Where(x => x.RequestId == requestId && x.EducationalResearchStatus == (int)EducationalResearchStatus.آموزشی).AsQueryable();
        }

        public IQueryable<EducationalResearch> GetReasearch_ByRequestId_ForCurrentYear(long requestId)
        {
            return _educationalResearches.Where(x => x.RequestId == requestId && x.EducationalResearchStatus == (int)EducationalResearchStatus.پژوهشی).AsQueryable();
        }

        public IQueryable<ScientificExecutive> GetScientificExecutive_ByRequestId_ForCurrentYear(long requestId)
        {
            return _scientificExecutives.Where(x => x.RequestId == requestId).AsQueryable();
        }

        public IQueryable<Technology> GetTechnology_ByRequestId_ForCurrentYear(long requestId)
        {
            return _technologies.Where(x => x.RequestId == requestId).AsQueryable();
        }

        public IQueryable<Dissertation> GetDissertation_ByRequestId_ForCurrentYear(long requestId)
        {
            return _dissertations.Where(x => x.RequestId == requestId).AsQueryable();
        }

        public IQueryable<AttachmentResearch> GetAttachmentResearches(long reaserchId)
        {
            return _attachmentResearches.Where(x => x.EducationaResearchId == reaserchId);
        }
        public AttachmentResearch FindAttachforResearch(long reaserchAttachId)
        {
            return _attachmentResearches.Find(reaserchAttachId);
        }
        public IQueryable<AttachmentFurtherInformation> GetAttachmentFurtherInformations(long furtherInformationId, int furtherInformationType)
        {
            return _attachmentFurtherInformations.Where(x => x.FurtherInformationId == furtherInformationId && x.FurtherInformationType == (FurtherInformationType)furtherInformationType);
        }

        public IQueryable<AttachmentFurtherInformation> GetAttachmentFurtherInformations(long furtherInformationId)
        {
            return _attachmentFurtherInformations.Where(x => x.FurtherInformationId == furtherInformationId);
        }
        public void EditRequest(Request request)
        {
            _requests.AddOrUpdate(c => c.Id, request);
        }

        public void EditFurtherInformation(FurtherInformation furtherInformation)
        {
            _furtherInformations.AddOrUpdate(c => c.Id, furtherInformation);
        }
        public void EditEducationalResearch(EducationalResearch educationalResearch)
        {
            _educationalResearches.AddOrUpdate(c => c.Id, educationalResearch);
        }
        public void EditDissertation(Dissertation dissertation)
        {
            _dissertations.AddOrUpdate(c => c.Id, dissertation);
        }
        public void EditScientificExecutive(ScientificExecutive scientificExecutive)
        {
            _scientificExecutives.AddOrUpdate(c => c.Id, scientificExecutive);
        }
        public void EditTechnology(Technology technology)
        {
            _technologies.AddOrUpdate(c => c.Id, technology);
        }
        public void AddOrUpdateTechnologyDetail(TechnologyDetail technologyDetail)
        {
            _technologyDetails.AddOrUpdate(c => c.Id, technologyDetail);
        }

    }
}
