using CommonLibrary;
using DataAccess;
using TCG_Scraper;

namespace CardDataAPI
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient((sp) => new Configuration(sp.GetService<IConfiguration>()));
            builder.Services.AddTransient<IApiLogger>((sp) => new ApiLogger());
            builder.Services.AddTransient<IRepositoryManager>((sp) => new RepositoryManager(sp.GetService<Configuration>().ConnectionSettings));

            var app = builder.Build();

            var config = new Configuration(app.Configuration);
            var repositoryManager = new RepositoryManager(config.ConnectionSettings);
            var logger = new ApiLogger();

            if (app.Environment.EnvironmentName != "Tests")
                InitScraper(repositoryManager, logger, config);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "DockerDev")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static async Task InitScraper(RepositoryManager repos, ApiLogger logger, Configuration config)
        {
            var scraper = new TcgScraper(repos, logger, config.ScraperSettings);
            var nextMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
            var cardRequester = new TcgCardRequester(logger);
            var productLines = await cardRequester.GetProductLines();

            productLines.ForEach(pLine =>
            {
                scraper.ExecuteAtIntervals(nextMidnight, TimeSpan.FromDays(1), pLine.ProductLineId);
                nextMidnight = nextMidnight.Add(TimeSpan.FromHours(1));
            });
        }
    }
}