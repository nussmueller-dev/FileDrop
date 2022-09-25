using FileDropBE.Database.Entities;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace FileDropBE.Attributes {
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizeAttribute : Attribute, IAuthorizationFilter {

    public AuthorizeAttribute() { }

    public void OnAuthorization(AuthorizationFilterContext context) {
      var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      var userLogic = context.HttpContext.RequestServices.GetService(typeof(UserLogic)) as UserLogic;
      var currentUserHelper = context.HttpContext.RequestServices.GetService(typeof(CurrentUserHelper)) as CurrentUserHelper;

      User user = userLogic.GetUserFromToken(token);

      if (user is null) {
        context.Result = new UnauthorizedResult();
        return;
      }

      currentUserHelper.SetCurrentUser(user);
    }
  }
}
