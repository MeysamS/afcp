using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Annual_faculty_promotions.WebUI.Helpers
{
    public static class GenericPrincipalExtensions
    {
        public static string FullName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "FullName") return claim.Value;
                    }
                return "";
            }
            else return "";
        }

        public static string FirstName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "FirstName") return claim.Value;
                    }
                return "";
            }
            else return "";
        }

        public static string LastName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "LastName") return claim.Value;
                    }
                return "";
            }
            else return "";
        }

        public static string Avatar(this IPrincipal user)
        {
            if ((user != null) && (user.Identity.IsAuthenticated))
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "Avatar") return claim.Value;
                    }
                return "";
            }
            else return "";
        }

        public static string UniversName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "UniversName") return claim.Value;
                    }
                return "";
            }
            else return "";
        }

        public static string Logo(this IPrincipal user)
        {
            if ((user != null) && (user.Identity.IsAuthenticated))
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.Type == "Logo") return claim.Value;
                    }
                return "";
            }
            else return "";
        }
    }
}