using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Models;
using DogsServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        protected bool IsCurrentUserMember()
        {
            var currentGuide = GetCurrentUser();
            if (currentGuide == null)
                return false;
            return currentGuide.IsMember;
        }
        protected bool IsCurrentUserAdmin()
        {
            var currentGuide = GetCurrentUser();
            if (currentGuide == null)
                return false;
            return currentGuide.IsAdmin;
        }
        protected int GetCurrentUserId()
        {
            if (User == null)
                return -1;
            var identity = ((System.Security.Claims.ClaimsIdentity)User.Identity);
            return Int32.Parse(identity.FindFirst("KgtId").Value);
        }
        protected Guide GetCurrentUser()
        {
            var id = GetCurrentUserId();
            if (id == -1)
                return null;
            return unitOfWork.GuideRepository.GetById(id);
        }
        protected string GetCurrentUserIdentityId()
        {
            if (User == null)
                return null;
            var identity = ((System.Security.Claims.ClaimsIdentity)User.Identity);
            return identity.FindFirst("IdentityId").Value;
        }

    }
}