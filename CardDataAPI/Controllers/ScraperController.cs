using CommonLibrary;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using TCG_Scraper;

namespace CardDataAPI.Controllers
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class ScraperController : BaseController
    {
        private Configuration Config { get; set; }
        public ScraperController(IRepositoryManager repoManager, IApiLogger logger, Configuration config) : base(repoManager, logger)
        {
            Config = config;
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> RefreshProductLine(int productLine)
        {
            var scraper = new TcgScraper(Repositories, Logger, Config.ScraperSettings);

            try
            {
                await scraper.LoadCardsByProductLine(productLine);
                return Ok("Data refreshed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return BadRequest("Unable to refresh data.");
            }
        }
    }
}
