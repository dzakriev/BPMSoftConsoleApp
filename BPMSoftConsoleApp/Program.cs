using System.Net;
using System.Text;

class Program
{
    private static readonly string searchString = "А";
    private const string authUrl = "http://localhost:5003/ServiceModel/AuthService.svc/Login";
    private static readonly string odataUrl = $"http://localhost:5003/odata/Account/$count?$filter=contains(Name, '{searchString}')";
    private static readonly string serviceUrl = $"http://localhost:5003/rest/UsrDataService/GetAccountsContainingTextCount?Name={searchString}";

    private static async Task<int> GetAccountsOData()
    {
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler { CookieContainer = cookieContainer };

        using var client = new HttpClient(handler);
        try
        {
            var authData = new
            {
                UserName = "Supervisor",
                UserPassword = "Supervisor"
            };
                
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(authData),
                Encoding.UTF8,
                "application/json");
                
            var authResponse = await client.PostAsync(authUrl, content);
            authResponse.EnsureSuccessStatusCode();
                
            var odataResponse = await client.GetAsync(odataUrl);
            odataResponse.EnsureSuccessStatusCode();
            var count = await odataResponse.Content.ReadAsStringAsync();

            return int.Parse(count);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Ошибка: {e.Message}\n StackTrace: {e.StackTrace}");
            throw;
        }
    }
    
    private static async Task<int> GetAccountsService()
    {
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler { CookieContainer = cookieContainer };

        using var client = new HttpClient(handler);
        try
        {
            var authData = new
            {
                UserName = "Supervisor",
                UserPassword = "Supervisor"
            };
                
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(authData),
                Encoding.UTF8,
                "application/json");
                
            var authResponse = await client.PostAsync(authUrl, content);
            authResponse.EnsureSuccessStatusCode();
                
            var serviceResponse = await client.GetAsync(serviceUrl);
            serviceResponse.EnsureSuccessStatusCode();
            var result = await serviceResponse.Content.ReadAsStringAsync();
            return int.Parse(result);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Ошибка: {e.Message}\n StackTrace: {e.StackTrace}");
            throw;
        }
    }

    public static async Task Main(string[] args)
    {
        var ODataResponse = await GetAccountsOData();
        Console.WriteLine($"OData: количество записей, содержащих \"{searchString}\": {ODataResponse}");

        var ServiceResponse = await GetAccountsService();
        Console.WriteLine($"Сервис: Количество записей, содержащих \"{searchString}\": {ServiceResponse}");
    }
}