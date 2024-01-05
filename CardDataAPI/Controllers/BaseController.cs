using CommonLibrary;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CardDataAPI.Controllers
{
    public class BaseController : Controller
    {
        protected IRepositoryManager Repositories { get; set; }
        protected IApiLogger Logger { get; set; }

        protected BaseController(IRepositoryManager repoManager, IApiLogger logger)
        {
            Repositories = repoManager;
            Logger = logger;
        }
    }
}
