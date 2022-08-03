using DogsServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IUserService UserService;

        public BaseController(IUserService userService)
        {
            UserService = userService;
        }
    }
}