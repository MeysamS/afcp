
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Postal;

namespace Annual_faculty_promotions.Service.Schedule
{
    //public class SendEmailSms : IJob
    //{
    //    private  IUnitOfWork _uow;
    //    private  IRequestService _requestService;
    //    private  ICartableService _cartableService;
    //    private  IUserService _userService;
    //    private  IArchiveService _archiveService;
    //    private  IDefinitionService _definitionService;


    //    public IList<AppUser> GetUsers()
    //    {
    //        //var archive = _archiveService.GetAllArchivesAsQueryable().GroupBy(s => s.User).Select(s => s.LastOrDefault()).ToList();

    //        var q = _userService.GetAllUsersAsQueryable()
    //            .Select(p => new
    //            {
    //                User = p,
    //                Request = p.Requests.LastOrDefault(),
    //                Request_Archive = p.Requests.LastOrDefault(x => x.Archive != null),
    //                profile = p.Profile
    //            });

    //        //var query = (from p in _userService.Where(w=>w.Profile.EmployeeDate!=null).ToList()
    //        //    select new
    //        //    {
    //        //        User=p,
    //        //        Request=p.Requests.LastOrDefault(),
    //        //        Request_Archive=p.Requests.LastOrDefault(x=>x.Archive!=null)
    //        //    }).AsEnumerable().ToList();

    //        var result = q.AsEnumerable().Select(x => x.User).ToList();

    //        return result;
    //    }

    //    public void Execute(IJobExecutionContext context)
    //    {
    //        try
    //        {

    //            //var archive = _archiveService.GetAllArchivesAsQueryable().GroupBy(s=>s.User).Select(s => s.LastOrDefault()).ToList();
    //            //var request = _requestService.GetAllRequestsAsQueryable().GroupBy(s => s.User).Select(s => s.LastOrDefault()).ToList();
    //            //var isArshiveList = request.Where(x => x.Archive == null).ToList();

    //            //var def = _definitionService.GetAllDefinitionsAsQueryable().Select(s=>new {s.DeadlineRequest,s.StartMail}).FirstOrDefault();
    //            //string[] defDate = def.DeadlineRequest.Split('/');
    //            //DateTime deadlineDate=new DateTime(int.Parse(defDate[0]),int.Parse(defDate[1]),int.Parse(defDate[2]));
    //            //DateTime dateNow = DateTime.Today;
    //            //dateNow.AddDays(Convert.ToDouble(def.StartMail));
    //            //if (dateNow == deadlineDate)
    //            //{
    //            //    var req = _requestService.Where(r => r.Cartables.Count > 0 && r.Archive == null).Include(i=>i.User).Select(s=>s.User).ToList();
    //            //    var user = _userService.GetAllUsersAsQueryable().Except(req).ToList();
    //            UsersEmail user = new UsersEmail(_uow, _cartableService, _archiveService, _userService, _requestService, _definitionService);
    //            var lstUser =user.GetUsers();
    //            EmailService emailService = new EmailService();
    //            MyEmail e = new MyEmail()
    //            {
    //                Email = "ali.kalij@yahoo.com",
    //                Body = "yes kalij",
    //                Subject = "ali"
    //            };
    //            //foreach (var item in user)
    //            //{

    //            //}
    //            emailService.Send(e);
    //            //    }
    //        }
    //        catch (Exception ex)
    //        {

    //            throw;
    //        }
    //    }

    //}

    public class UsersEmail
    {
        private readonly IUnitOfWork _uow;
        private readonly IRequestService _requestService;
        private readonly ICartableService _cartableService;
        private readonly IUserService _userService;
        private readonly IArchiveService _archiveService;
        private readonly IDefinitionService _definitionService;


        public UsersEmail(IUnitOfWork uow, ICartableService cartableService, IArchiveService archiveService, IUserService userService, IRequestService requestService, IDefinitionService definitionService)
        {
            _uow = uow;
            _cartableService = cartableService;
            _archiveService = archiveService;
            _userService = userService;
            _requestService = requestService;
            _definitionService = definitionService;
        }

        public IList<AppUser> GetUsers()
        {
            //var archive = _archiveService.GetAllArchivesAsQueryable().GroupBy(s => s.User).Select(s => s.LastOrDefault()).ToList();
            
            var q = _userService.GetAllUsersAsQueryable()
                .Select(p => new
                {
                    User = p,
                    Request = p.Requests.LastOrDefault(),
                    Request_Archive = p.Requests.LastOrDefault(x => x.Archive != null),
                    profile=p.Profile
                });

            //var query = (from p in _userService.Where(w=>w.Profile.EmployeeDate!=null).ToList()
            //    select new
            //    {
            //        User=p,
            //        Request=p.Requests.LastOrDefault(),
            //        Request_Archive=p.Requests.LastOrDefault(x=>x.Archive!=null)
            //    }).AsEnumerable().ToList();

            var result = q.AsEnumerable().Select(x => x.User).ToList();
            
            return result;

            var request = _requestService.GetAllRequestsAsQueryable().GroupBy(s => s.User).Select(s => s.LastOrDefault()).ToList();
            var isArshiveList = request.Where(x => x.Archive == null).ToList();

            //var def = _definitionService.GetAllDefinitionsAsQueryable().Select(s=>new {s.DeadlineRequest,s.StartMail}).FirstOrDefault();
            //string[] defDate = def.DeadlineRequest.Split('/');
            //DateTime deadlineDate=new DateTime(int.Parse(defDate[0]),int.Parse(defDate[1]),int.Parse(defDate[2]));
            //DateTime dateNow = DateTime.Today;
            //dateNow.AddDays(Convert.ToDouble(def.StartMail));
            //if (dateNow == deadlineDate)
            //{
            //    var req =
            //        _requestService.Where(r => r.Cartables.Count > 0 && r.Archive == null)
            //            .Include(i => i.User)
            //            .Select(s => s.User)
            //            .ToList();
            //    var user = _userService.GetAllUsersAsQueryable().Except(req).ToList();
            //}
            //return archive;
        }
    }
}