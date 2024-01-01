using ApiModels;
using DataAccess;
using DataAccess.Repositories;
using DataAccess.SqlModels;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

string productLinesUrl = "https://mp-search-api.tcgplayer.com/v1/search/productLines?";
string setListUrl = "https://mpapi.tcgplayer.com/v2/Catalog/SetNames?active=true&categoryId=";
string cardListUrl = "https://mp-search-api.tcgplayer.com/v1/search/request?";
string productLineName = "Flesh and Blood TCG";
string saveDataFileName = "cardData_fb.json";
int productLineId = 0;
int cardsPerRequest = 48;
JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
HttpClient client = new();
bool skipScrape = true;

long startTime = Stopwatch.GetTimestamp();

List<CardInfo> cardData = new();

if (!skipScrape || !File.Exists(saveDataFileName))
{
    Console.WriteLine("Requesting Product Lines...");

    var response = await client.GetAsync(productLinesUrl);

    if (!response.IsSuccessStatusCode)
        throw new Exception("Product Lines request failed.");

    Console.WriteLine("Product Lines request succeeded.");

    var responseStream = await response.Content.ReadAsStreamAsync();
    var productLines = JsonSerializer.Deserialize<List<ProductLine>>(responseStream, options);
    var productLine = productLines?.FirstOrDefault(p => p.ProductLineName == productLineName) ?? throw new Exception("Product Line not found.");
    var productLineUrlName = productLine.ProductLineUrlName;
    productLineId = productLine.ProductLineId;

    Console.WriteLine("Requesting Set list...");

    response = await client.GetAsync(setListUrl + productLineId.ToString());

    if (!response.IsSuccessStatusCode)
        throw new Exception("Set list request failed.");

    Console.WriteLine("Set list request succeeded.");

    responseStream = await response.Content.ReadAsStreamAsync();
    var sets = JsonSerializer.Deserialize<RequestResponse<SetInfo>>(responseStream, options) ?? throw new Exception("Set data could not be deserialized.");

    //TODO Search api with no set name to get total cards for product line and throw error if any set search returns all cards (incorrect set name).


    foreach (var set in sets.Results/*.Where(s => !setNamesPulled.Contains(s.Name))*/)
    {
        var setSearchName = set.CleanSetName.ToLower().Replace(" ", "-");

        RequestPayload requestPayload = new(productLineUrlName, setSearchName, 0, cardsPerRequest);

        JsonContent searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

        Console.WriteLine($"Requesting Card Info 0-{cardsPerRequest - 1} for set '{setSearchName}'...");

        RequestResponse<CardInfoResults> cardInfos = new();

        try
        {
            response = await client.PostAsync(cardListUrl, searchContent);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Card List request failed.");

            responseStream = await response.Content.ReadAsStreamAsync();

            cardInfos = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine("err");
        }

        var setCards = cardInfos.Results[0].Results;
        int totalCards = cardInfos.Results[0].TotalResults;

        for (int i = cardsPerRequest; i < totalCards; i += cardsPerRequest)
        {
            requestPayload = new(productLineUrlName, setSearchName, i, cardsPerRequest);
            searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

            Console.WriteLine($"Requesting Card Info {i}-{i + cardsPerRequest - 1} for set '{setSearchName}'...");

            try
            {
                response = await client.PostAsync(cardListUrl, searchContent);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Card List request failed.");

                responseStream = await response.Content.ReadAsStreamAsync();
                cardInfos = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
                setCards = setCards.Concat(cardInfos.Results[0].Results).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("err");
            }
        }

        cardData = cardData.Concat(setCards).ToList();
    }
}
else
{
    cardData = JsonSerializer.Deserialize<List<CardInfo>>(File.OpenRead(saveDataFileName));
    productLineId = cardData[0].ProductLineId.AsInt();
}

var cardInfoProps = typeof(CardInfo).GetProperties().Where(prop => prop.Name is not ("CustomAttributes" or "Listings"));
var listingProps = typeof(CardListing).GetProperties();

if (!skipScrape || !File.Exists(saveDataFileName))
    File.WriteAllText(saveDataFileName, JsonSerializer.Serialize(cardData));

var uniqueCardData = cardData.DistinctBy(data => data.ProductId);
var cards = uniqueCardData.Select(data => new Card().AddMatchingPropertyValues(data)).ToList();

Cards.ImportCards(cards);

var customAtts = uniqueCardData.SelectMany(data => data.CustomAttributes.Where(att => att.Value.ValueKind != JsonValueKind.Null))
    .DistinctBy(att => att.Key)
    .Select(att => new CustomAttribute()
    {
        Name = att.Key,
        DisplayName = Regex.Replace(att.Key, "(?<l>[a-z])(?<u>[A-Z])", "${l} ${u}"),
        DataType = att.Value.ValueKind.ToString(),
        ProductLineId = productLineId
    });

CustomAttributes.ImportCustomAttributes(customAtts);
var customAttsDic = CustomAttributes.GetAttributesByProductLine(productLineId)
    .ToDictionary(att => att.Name, att => att.CustomAttributeId);

var attValues = uniqueCardData
    .SelectMany(data => data.CustomAttributes
    .Where(atts => atts.Value.ValueKind != JsonValueKind.Null)
    .Select(atts => new CustomAttributesValue()
    {
        ProductId = data.ProductId.AsInt(),
        CustomAttributeId = customAttsDic[atts.Key],
        Value = atts.Value.AsString()
    }));

CustomAttributesValues.ImportCustomAttributesValues(attValues);

Console.WriteLine($"Finished in: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms");