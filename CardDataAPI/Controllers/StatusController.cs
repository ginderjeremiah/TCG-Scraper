using CardDataAPI.ResponseModels;
using CommonLibrary;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CardDataAPI.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class StatusController : BaseController
    {
        public StatusController(IRepositoryManager repoManager, IApiLogger logger) : base(repoManager, logger) { }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return Status();
        }

        [HttpGet]
        public IActionResult Status()
        {
            return Json(new StatusResponse()
            {
                Host = HttpContext.Request.Host.Value,
                Status = "Available"
            });
        }
    }
}
