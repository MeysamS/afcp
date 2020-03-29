using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Helpers.Util;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Annual_faculty_promotions.WebUI.Hubs
{
    [HubName("cartableHub")]
    public class CartableHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IMessagingService _messagingService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ICartableService _cartableService;
        public CartableHub(IRequestService requestService, ICartableService cartableService, IMessagingService messagingService, IUserService userService, IApplicationRoleManager roleManager)
        {
            _requestService = requestService;
            _userService = userService;
            _roleManager = roleManager;
            _messagingService = messagingService;
            _cartableService = cartableService;
        }
        public void Join()
        {
            var userName = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;
            var user = Users.GetOrAdd(userName, _ => new User
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>(),
                MaRole = Context.User.IsInRole("MA"),
                MgRole = Context.User.IsInRole("MG"),
                RdRole = Context.User.IsInRole("RD"),
                RkRole = Context.User.IsInRole("RK")
            });

            if (user.MaRole) { Groups.Add(user.ConnectionIds.ToString(), "MA"); }
            else
                if (user.MgRole) { Groups.Add(user.ConnectionIds.ToString(), "MG"); }
                else
                    if (user.RdRole) { Groups.Add(user.ConnectionIds.ToString(), "RD"); }
                    else
                        if (user.RkRole)
                        {
                            Groups.Add(user.ConnectionIds.ToString(), "RK");
                        }
                        else
                        {
                            lock (user.ConnectionIds)
                            {
                                user.ConnectionIds.Add(connectionId);
                            }
                        }
        }

        public void UnJoin()
        {
            var userName = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;
            User user;
            Users.TryGetValue(userName, out user);
            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        User removeUser;
                        Users.TryRemove(userName, out removeUser);
                    }
                }
            }
        }

        public void AddNewCartableNotification(int uidRecive,int uidSender)
        {

            //var roles = _roleManager.FindUserRoles(int.Parse(uid));
            //if (roles!=null)
            //    foreach (var item in roles)
            //    {
            //        Clients.Group(item.Name).sendNewCartable("true");   
            //    }      

            Clients.All.sendNewCartable(uidRecive);
            Clients.Others.getCartableCount(_cartableService.GetAllCartablesAsQueryable().Count(x => x.Active == true && x.UserSenderId == uidSender));
            Clients.Client(Context.ConnectionId).getCartableCount(_cartableService.GetAllCartablesAsQueryable().Count(x => x.Active == true && x.UserReciveId == uidRecive));
        }


        public void AddNewMessageNotification(string messageId)
        {
            int msgid = int.Parse(messageId);
            var model = _messagingService.Where(x => x.Id == msgid)
                .Include(x => x.UserSender)
                .Include(x => x.UserSender.Profile).Include(x => x.UserReciever)
                .Include(x => x.UserReciever.Profile).FirstOrDefault();
            if (model == null)
                return;

            string strHtml = "<div class='message'><img src='/Content/Images/Avatars/" + model.UserSender.Profile.Avatar + "'" + model.UserSender.Profile.Name + " " + model.UserSender.Profile.Family + "  class='message-avatar'>" +
                      " <a href='#' class='message-subject'>" + model.Text + "</a>" +
                      "<div class='message-description'>" +
                      "فرستنده:<a href='#' title=''>" + model.UserSender.Profile.Name + " " + model.UserSender.Profile.Family + "</a>|" +
                      "گیرنده <a href='#' title=''>" + model.UserReciever.Profile.Name + " " + model.UserReciever.Profile.Family + "</a>" +
                      "&nbsp;&nbsp;·&nbsp;&nbsp;" +
                      RelativeTimeCalculator.Calculate(model.CreatedDate) +
                      "</div>" +
                      "</div> ";
            Clients.Others.sendNotification(strHtml);
            
            Clients.Others.sendMessage();
        }
    }

    public class User
    {
        public string UserName { get; set; }
        public bool RkRole { get; set; }
        public bool MaRole { get; set; }
        public bool MgRole { get; set; }
        public bool RdRole { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}