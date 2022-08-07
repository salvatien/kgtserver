using DogsServer.Repositories;
using System;
using System.Security.Claims;
using Dogs.Data.Models;
using Dogs.Data.DbContexts;

namespace DogsServer.Services
{
    public interface IUserService
    {
        public bool IsCurrentUserMember(ClaimsPrincipal User);
        public bool IsCurrentUserAdmin(ClaimsPrincipal User);
        public int GetCurrentUserId(ClaimsPrincipal User);
        public Guide GetCurrentUser(ClaimsPrincipal User);
        public string GetCurrentUserIdentityId(ClaimsPrincipal User);
    }
    public class UserService : IUserService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public UserService(AppDbContext dbContext)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        public bool IsCurrentUserMember(ClaimsPrincipal User)
        {
            var currentGuide = GetCurrentUser(User);
            if (currentGuide == null)
                return false;
            return currentGuide.IsMember;
        }
        public bool IsCurrentUserAdmin(ClaimsPrincipal User)
        {
            var currentGuide = GetCurrentUser(User);
            if (currentGuide == null)
                return false;
            return currentGuide.IsAdmin;
        }
        public int GetCurrentUserId(ClaimsPrincipal User)
        {
            if (User == null)
                return -1;
            var identity = (ClaimsIdentity)User.Identity;
            return Int32.Parse(identity.FindFirst("KgtId").Value);
        }
        public Guide GetCurrentUser(ClaimsPrincipal User)
        {
            var id = GetCurrentUserId(User);
            if (id == -1)
                return null;
            return unitOfWork.GuideRepository.GetById(id);
        }
        public string GetCurrentUserIdentityId(ClaimsPrincipal User)
        {
            if (User == null)
                return null;
            var identity = (ClaimsIdentity)User.Identity;
            return identity.FindFirst("IdentityId").Value;
        }
    }
}
