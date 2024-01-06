using CardDataAPI;
using DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Mocks
{
    internal class ApiAppFactory : WebApplicationFactory<Startup>
    {
        public MockLogger Logger { get; set; } = new MockLogger();
        public MockRepositoryManager Repositories { get; set; } = new MockRepositoryManager();
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Tests");

            builder.ConfigureTestServices(services =>
            {
                //services.AddTransient((sp) => new Configuration(sp.GetService<IConfiguration>()));
                services.AddSingleton<IApiLogger>(Logger);
                services.AddSingleton<IRepositoryManager>(Repositories);
            });
        }
    }
}
