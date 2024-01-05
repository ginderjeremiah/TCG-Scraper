using CommonLibrary;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CardDataAPI.Controllers
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class CardsController : BaseController
    {
        public CardsController(IRepositoryManager repoManager, IApiLogger logger) : base(repoManager, logger) { }

        [Route("/api/Cards")]
        [HttpGet]
        public IActionResult Cards(int? offset, int? limit)
        {
            return Json(Repositories.Cards.GetAllCards(offset ?? 0, limit ?? 100));
        }
    }
}
