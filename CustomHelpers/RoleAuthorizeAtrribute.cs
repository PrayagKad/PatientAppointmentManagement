using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;

namespace AppointmentBooking.CustomHelpers
{

    // acts as a validation for roles
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {

        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        // authorize the role 
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["UserId"] == null)
            {
                return false;
            }

            string role = httpContext.Session["Role"]?.ToString();

            return _roles.Contains(role);
        }
    }

}