using CardDataAPI.ResponseModels;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class StatusControllerTests
    {
        [TestMethod]
        public async Task Index_NormalUsage_ReturnsSuccess()
        {
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();

            var response = await client.GetAsync("/");

            var data = response.Deserialize<StatusResponse>();

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Status == "Available");
        }

        [TestMethod]
        public async Task Status_NormalUsage_ReturnsSuccess()
        {
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();

            var response = await client.GetAsync("/api/Status");

            var data = response.Deserialize<StatusResponse>();

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Status == "Available");
        }
    }
}
