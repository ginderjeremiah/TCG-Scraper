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

long startTime = Stopwatch.GetTimestamp();

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
List<CardInfo> cardData = new();

foreach (var set in sets.Results)
{
    RequestPayload requestPayload = new(productLineUrlName, set.UrlName, 0, cardsPerRequest);

    JsonContent searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

    Console.WriteLine($"Requesting Card Info 0-{cardsPerRequest - 1} for set '{set.UrlName}'...");

    response = await client.PostAsync(cardListUrl, searchContent);

    if (!response.IsSuccessStatusCode)
        throw new Exception("Card List request failed.");

    responseStream = await response.Content.ReadAsStreamAsync();

    var cards = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
    cardData = cardData.Concat(cards.Results[0].Results).ToList();
    int totalCards = cards.Results[0].TotalResults;

    for (int i = cardsPerRequest; i < totalCards; i += cardsPerRequest)
    {
        requestPayload = new(productLineUrlName, set.UrlName, i, cardsPerRequest);
        searchContent = JsonContent.Create(requestPayload, new MediaTypeHeaderValue("application/json"), options);

        Console.WriteLine($"Requesting Card Info {i}-{i + cardsPerRequest - 1} for set '{set.UrlName}'...");

        response = await client.PostAsync(cardListUrl, searchContent);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Card List request failed.");

        responseStream = await response.Content.ReadAsStreamAsync();
        cards = JsonSerializer.Deserialize<RequestResponse<CardInfoResults>>(responseStream, options);
        cardData = cardData.Concat(cards.Results[0].Results).ToList();
    }
}

var cardInfoProps = typeof(CardInfo).GetProperties().Where(prop => prop.Name is not ("CustomAttributes" or "Listings"));
var customAttProps = typeof(CustomAttributes).GetProperties();
var listingProps = typeof(CardListing).GetProperties();

File.WriteAllText("cardData.json", JsonSerializer.Serialize(cardData));

Console.WriteLine("Writing cardInfo.csv...");

var namedData = cardData.DistinctBy(data => data.ProductUrlName).ToDictionary(data => data.ProductUrlName, data => data);
var numData = cardData.DistinctBy(data => data.ProductId).ToDictionary(data => data.ProductId, data => data);

File.WriteAllText("cardDataV2.json", JsonSerializer.Serialize(numData));

var invalidNames = cardData.Select(d => d.ProductUrlName).ToList();

var cardInfoCsv = cardData.Select(data => string.Join(",", cardInfoProps.Select(prop => GetValue(prop, data)).Concat(customAttProps.Select(prop => GetValue(prop, data.CustomAttributes)))));
cardInfoCsv = cardInfoCsv.Prepend(string.Join(",", cardInfoProps.Select(prop => prop.Name).Concat(customAttProps.Select(prop => prop.Name))));

File.WriteAllLines("cardInfo.csv", cardInfoCsv);

Console.WriteLine("Writing listingInfo.csv...");

var listingInfoCsv = cardData.SelectMany(data => data.Listings).Select(listing => string.Join(",", listingProps.Select(prop => GetValue(prop, listing))));
listingInfoCsv = listingInfoCsv.Prepend(string.Join(",", listingProps.Select(prop => prop.Name)));

File.WriteAllLines("listingInfo.csv", listingInfoCsv);

Console.WriteLine($"Finished in: {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms");

Console.WriteLine("done");

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

class ProductLine
{
    public int ProductLineId { get; set; }
    public string? ProductLineName { get; set; }
    public string? ProductLineUrlName { get; set; }
}

class RequestPayload
{
    //public string Algorithm { get; set; } = "sales_exp_fields_experiment";
    //public RequestContext Context { get; set; }
    public RequestFilters Filters { get; set; }
    public int From { get; set; } = 0;
    //public RequestListingSearch ListingSearch { get; set; }
    //public RequestSettings Settings { get; set; }
    public int Size { get; set; } = 24;
    //public object Sort { get; set; }

    public RequestPayload(string productLineName, string setName, int start, int size)
    {
        Filters = new RequestFilters(productLineName, setName);
        Size = size;
        From = start;
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

class RequestFilters
{
    //public object Match { get; set; } = new();
    //public object Range { get; set; } = new();
    public RequestFilterTerm Term { get; set; }
    public RequestFilters(string productLineName, string setName)
    {
        Term = new RequestFilterTerm(productLineName, setName);
    }
}

class RequestFilterTerm
{
    public List<string> ProductLineName { get; set; }
    public List<string> SetName { get; set; }
    public RequestFilterTerm(string productLineName, string setName)
    {
        ProductLineName = new List<string> { productLineName };
        SetName = new List<string> { setName };
    }
}

class RequestResponse<T>
{
    public List<string> Errors { get; set; }
    public List<T> Results { get; set; }

}

class CardInfoResults
{
    //public RequestResultsAggregations Aggregations { get; set; }
    //public string Algorithm { get; set; }
    //public string SearchType { get; set; }
    //public object DidYouMean { get; set; }
    public int TotalResults { get; set; }
    //public string ResultId { get; set; }
    public List<CardInfo> Results { get; set; }
}

class CardInfo
{
    public float ShippingCategoryId { get; set; }
    public bool Duplicate { get; set; }
    public string ProductLineUrlName { get; set; }
    public string ProductUrlName { get; set; }
    public float ProductTypeId { get; set; }
    public string RarityName { get; set; }
    public bool Sealed { get; set; }
    public float MarketPrice { get; set; }
    public CustomAttributes CustomAttributes { get; set; }
    public float LowestPriceWithShipping { get; set; }
    public string ProductName { get; set; }
    public float SetId { get; set; } //not sure why this isn't an int
    public float ProductId { get; set; } //not sure why this isn't an int
    public float Score { get; set; }
    public string SetName { get; set; }
    public bool FoilOnly { get; set; }
    public string SetUrlName { get; set; }
    public bool SellerListable { get; set; }
    public float TotalListings { get; set; } //not sure why this isn't an int
    public float ProductLineId { get; set; } //not sure why this isn't an int
    public float ProductStatusId { get; set; } //not sure why this isn't an int
    public string ProductLineName { get; set; }
    public float MaxFulfullableQuantity { get; set; }
    public List<CardListing> Listings { get; set; }
    public float LowestPrice { get; set; }
}

class CustomAttributes
{
    public string Description { get; set; }
    public string DetailNote { get; set; }
    public string? Intellect { get; set; } //not sure what data type this should be... possibly List<string>
    public DateTime ReleaseDate { get; set; }
    public string Number { get; set; }
    public List<string> Talent { get; set; }
    public string PitchValue { get; set; }
    public List<string> CardType { get; set; }
    public string DefenseValue { get; set; }
    public string RarityDbName { get; set; }
    public string? Life { get; set; } //not sure what data type this should be... possibly List<string>
    public List<string> CardSubType { get; set; }
    public string Power { get; set; }
    public string? FlavorText { get; set; }
    public List<string> Class { get; set; }
    public string Cost { get; set; }
}

class CardListing
{
    public bool DirectProduct { get; set; }
    public bool GoldSeller { get; set; }
    public float ListingId { get; set; } //not sure why this isn't an int
    public float ChannelId { get; set; } //not sure why this isn't an int
    public float ConditionId { get; set; } //not sure why this isn't an int
    public bool VerifiedSeller { get; set; }
    public float DirectInventory { get; set; }
    public float RankedShippingPrice { get; set; }
    public float ProductId { get; set; } //not sure why this isn't an int
    public string Printing { get; set; }
    public string LanguageAbbreviation { get; set; }
    public string SellerName { get; set; }
    public bool ForwardFreight { get; set; }
    public float SellerShippingPrice { get; set; }
    public string Language { get; set; }
    public float ShippingPrice { get; set; }
    public string Condition { get; set; }
    public float LanguageId { get; set; } //not sure why this isn't an int
    public float Score { get; set; }
    public bool DirectSeller { get; set; }
    public float ProductConditionId { get; set; } //not sure why this isn't an int
    public string SellerId { get; set; } //not sure why this isn't an int
    public string ListingType { get; set; }
    public float SellerRating { get; set; }
    public string SellerSales { get; set; }
    public float Quantity { get; set; } //not sure why this isn't an int
    public string SellerKey { get; set; }
    public float Price { get; set; }
    public CustomListingData CustomData { get; set; }
}

class CustomListingData
{
    public List<string> Images { get; set; } //not sure what appropriate data type is... assuming these are img URLs

    public override string ToString()
    {
        return string.Join(", ", Images);
    }
}

class SetInfo
{
    public int SetNameId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string CleanSetName { get; set; }
    public string UrlName { get; set; }
    public string Abbreviation { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool Active { get; set; }
}


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