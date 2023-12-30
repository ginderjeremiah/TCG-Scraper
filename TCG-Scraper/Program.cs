using ApiModels;
using DataAccess;
using DataAccess.Repositories;
using DataAccess.SqlModels;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

string productLinesUrl = "https://mp-search-api.tcgplayer.com/v1/search/productLines?";
string setListUrl = "https://mpapi.tcgplayer.com/v2/Catalog/SetNames?active=true&categoryId=";
string cardListUrl = "https://mp-search-api.tcgplayer.com/v1/search/request?";
string productLineName = "Flesh and Blood TCG";
int cardsPerRequest = 48;
JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
HttpClient client = new();
bool skipScrape = false;

long startTime = Stopwatch.GetTimestamp();

List<CardInfo> cardData = new();

if (!skipScrape)
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
    var productLineId = productLine.ProductLineId;

    Console.WriteLine("Requesting Set list...");

    response = await client.GetAsync(setListUrl + productLineId.ToString());

    if (!response.IsSuccessStatusCode)
        throw new Exception("Set list request failed.");

    Console.WriteLine("Set list request succeeded.");

    responseStream = await response.Content.ReadAsStreamAsync();
    var sets = JsonSerializer.Deserialize<RequestResponse<SetInfo>>(responseStream, options) ?? throw new Exception("Set data could not be deserialized.");


    foreach (var set in sets.Results)
    {
        RequestPayload requestPayload = new(productLineUrlName, set.UrlName, 0, cardsPerRequest);

        JsonContent searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

        Console.WriteLine($"Requesting Card Info 0-{cardsPerRequest - 1} for set '{set.UrlName}'...");

        response = await client.PostAsync(cardListUrl, searchContent);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Card List request failed.");

        responseStream = await response.Content.ReadAsStreamAsync();

        var cardInfos = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
        cardData = cardData.Concat(cardInfos.Results[0].Results).ToList();
        int totalCards = cardInfos.Results[0].TotalResults;

        for (int i = cardsPerRequest; i < totalCards; i += cardsPerRequest)
        {
            requestPayload = new(productLineUrlName, set.UrlName, i, cardsPerRequest);
            searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

            Console.WriteLine($"Requesting Card Info {i}-{i + cardsPerRequest - 1} for set '{set.UrlName}'...");

            response = await client.PostAsync(cardListUrl, searchContent);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Card List request failed.");

            responseStream = await response.Content.ReadAsStreamAsync();
            cardInfos = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
            cardData = cardData.Concat(cardInfos.Results[0].Results).ToList();
        }
    }
}
else
{
    cardData = JsonSerializer.Deserialize<List<CardInfo>>(File.ReadAllText("cardData.json"));
}


var cardInfoProps = typeof(CardInfo).GetProperties().Where(prop => prop.Name is not ("CustomAttributes" or "Listings"));
var customAttProps = typeof(CustomAttributes).GetProperties();
var listingProps = typeof(CardListing).GetProperties();

File.WriteAllText("cardData.json", JsonSerializer.Serialize(cardData));

var cards = cardData.DistinctBy(data => data.ProductId).Select(data => new Card().AddMatchingPropertyValues(data).AddMatchingPropertyValues(data.CustomAttributes)).ToList();

Cards.ImportCards(cards);

//using var connection = new NpgsqlConnection("Copy data ()");
//connection.BeginBinaryImport("");     

//File.WriteAllText("cardData.json", JsonSerializer.Serialize(cardData));

//Console.WriteLine("Writing cardInfo.csv...");

//var namedData = cardData.DistinctBy(data => data.ProductUrlName).ToDictionary(data => data.ProductUrlName, data => data);
//var numData = cardData.DistinctBy(data => data.ProductId).ToDictionary(data => data.ProductId, data => data);

//File.WriteAllText("cardDataV2.json", JsonSerializer.Serialize(numData));

//var invalidNames = cardData.Select(d => d.ProductUrlName).ToList();

//var cardInfoCsv = cardData.Select(data => string.Join(",", cardInfoProps.Select(prop => GetValue(prop, data)).Concat(customAttProps.Select(prop => GetValue(prop, data.CustomAttributes)))));
//cardInfoCsv = cardInfoCsv.Prepend(string.Join(",", cardInfoProps.Select(prop => prop.Name).Concat(customAttProps.Select(prop => prop.Name))));

//File.WriteAllLines("cardInfo.csv", cardInfoCsv);

//Console.WriteLine("Writing listingInfo.csv...");

//var listingInfoCsv = cardData.SelectMany(data => data.Listings).Select(listing => string.Join(",", listingProps.Select(prop => GetValue(prop, listing))));
//listingInfoCsv = listingInfoCsv.Prepend(string.Join(",", listingProps.Select(prop => prop.Name)));

//File.WriteAllLines("listingInfo.csv", listingInfoCsv);

Console.WriteLine($"Finished in: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms");

string GetValue(PropertyInfo prop, object obj)
{
    var val = prop.GetValue(obj);
    if (val == null)
    {
        return "null";
    }
    else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
    {
        var valList = (IEnumerable<object>)val;
        return $"\"{string.Join(",", valList.Select(v => v?.ToString()?.Replace("\"", "\"\"") ?? "null"))}\"";
    }
    else
    {
        return $"\"{val.ToString().Replace("\"", "\"\"")}\"";
    }
}




//class RequestContext
//{
//    public RequestContextCart Cart { get; set; } = new();
//    public string ShippingCountry { get; set; } = "US";
//    public RequestUserProfile UserProfile { get; set; } = new();
//}

//class RequestContextCart
//{
//    //empty obj
//}

//class RequestUserProfile
//{
//    public string? ProductLineAffinity { get; set; } = null;
//}

//class PropTree<T>
//{
//    public List<PropTreeNode> Children { get; set; }

//    public PropTree(T obj)
//    {
//        var baseProps = typeof(T).GetProperties().Where(prop => prop.CanWrite);
//        Children = new List<PropTreeNode>();
//        foreach (var prop in baseProps)
//        {
//            Children.Add(MakePropTreeNode(prop));
//        }
//    }

//    private PropTreeNode MakePropTreeNode(PropertyInfo propertyInfo, PropTreeNode? parent = null)
//    {
//        var props = propertyInfo.PropertyType.GetProperties().Where(prop => prop.CanWrite);
//        var node = new PropTreeNode()
//        {
//            Children = new List<PropTreeNode>(),
//            Prop = propertyInfo,
//            Parent = parent
//        };

//        foreach (var prop in props)
//        {
//            node.Children.Add(MakePropTreeNode(prop, node));
//        }

//        return node;
//    }

//    public List<string> GetPropNames()
//    {
//        List<string> names = new();
//        foreach (var node in Children)
//        {
//            names = names.Concat(GetNodeNames(node)).ToList();
//        }
//        return names;
//    }

//    private List<string> GetNodeNames(PropTreeNode node)
//    {
//        List<string> names = new() { node.Prop.Name };
//        foreach (var child in Children)
//        {
//            names = names.Concat(GetNodeNames(child)).ToList();
//        }
//        return names;
//    }

//    public List<string> GetPropValues(T obj)
//    {
//        List<string> values = new();
//        foreach (var node in Children)
//        {
//            values = values.Concat(GetNodeValues(node, obj)).ToList();
//        }
//        return values;
//    }

//    private List<string> GetNodeValues(PropTreeNode node, T obj)
//    {
//        List<string> values = new() { node.Prop.GetValue(obj)?.ToString() ?? "null" };
//        foreach (var child in Children)
//        {
//            values = values.Concat(GetNodeValues(node, obj)).ToList();
//        }
//        return values;
//    }
//}

//class PropTreeNode
//{
//    public List<PropTreeNode> Children { get; set; }
//    public PropertyInfo Prop { get; set; }
//    public PropTreeNode? Parent { get; set; }
//}