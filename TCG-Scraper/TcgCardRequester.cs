﻿using ApiModels;
using CommonLibrary;
using System.Net.Http.Json;

namespace TCG_Scraper
{
    public class TcgCardRequester
    {
        private const string _productLinesUrl = "https://mp-search-api.tcgplayer.com/v1/search/productLines?";
        private const string _setListUrl = "https://mpapi.tcgplayer.com/v2/Catalog/SetNames?active=true&categoryId=";
        private const string _cardListUrl = "https://mp-search-api.tcgplayer.com/v1/search/request?";

        private static HttpClient Client { get; } = new();

        private IApiLogger Logger { get; set; }

        public TcgCardRequester(IApiLogger logger)
        {
            Logger = logger;
        }

        public async Task<List<ProductLine>> GetProductLines()
        {
            Logger.Log("Requesting Product Lines...");

            var response = await Client.GetAsync(_productLinesUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Product Lines request failed.");

            Logger.Log("Product Lines request succeeded.");

            return response.Deserialize<List<ProductLine>>()
                ?? throw new Exception("Failed to parse Product Lines request.");
        }

        public async Task<List<SetInfo>> GetSets(int productLineId)
        {
            Logger.Log("Requesting Set list...");

            var response = await Client.GetAsync(_setListUrl + productLineId.ToString());

            if (!response.IsSuccessStatusCode)
                throw new Exception("Set list request failed.");

            Logger.Log("Set list request succeeded.");


            var setsResponse = response.Deserialize<RequestResponse<SetInfo>>()
                ?? throw new Exception("Set data could not be deserialized.");

            return setsResponse.Results;
        }

        public async Task<int> GetTotalCardsForProductLine(string productLineUrlName)
        {
            return (await RequestCardInfos(productLineUrlName, "", 0, 0)).TotalResults;
        }

        public async Task<CardInfoResults> RequestCardInfos(string productLineUrlName, string setSearchName, int skip, int totalResults)
        {
            var requestPayload = new RequestPayload(productLineUrlName, setSearchName, skip, totalResults);

            JsonContent searchContent = requestPayload.Serialize();

            Logger.Log($"Requesting Card Info {requestPayload.From}-{requestPayload.From + requestPayload.Size - 1} for set {requestPayload.Filters.Term.SetName[0]}");

            try
            {
                var response = await Client.PostAsync(_cardListUrl, searchContent);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Card List request failed.");

                var result = response.Deserialize<RequestResponse<CardInfoResults>>()
                    ?? throw new Exception("Card info data could not be deserialized.");

                if (result.Results.Count < 1)
                    throw new Exception("Card info data response contained no results.");

                return result.Results[0];
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return new CardInfoResults()
                {
                    TotalResults = 0,
                    Results = new()
                };
            }
        }
    }
}
