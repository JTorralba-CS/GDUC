using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


var Domain = "Your_GoDaddy_Domain_Name";
var Data = "0.0.0.0";
var Name = "@";
var TTL = 600;
var Type = "A";

var Key = "Your_GoDaddy_Developer_API_Key";
var Secret = "Your_GoDaddy_Developer_API_Secret";
var SSO_Key = Key + ":" + Secret;
var Method = "v1/domains/" + Domain + "/records/" + Type + "/" + Name;


var Client_GoDaddy = new HttpClient()
{
    BaseAddress = new Uri("https://api.godaddy.com")
};

Client_GoDaddy.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("sso-key", SSO_Key);

var Response = await Client_GoDaddy.GetStringAsync(Method);

using JsonDocument JSON = JsonDocument.Parse(Response);
JsonElement Elements = JSON.RootElement;

var Record = Elements[0];

var Current = new Record(Record.GetProperty("data").ToString(), Record.GetProperty("name").ToString(), int.Parse(Record.GetProperty("ttl").ToString()), Record.GetProperty("type").ToString());
Console.WriteLine(Current.data);
Console.WriteLine(Current.name);
Console.WriteLine(Current.ttl);
Console.WriteLine(Current.type);
Console.WriteLine();


var Client_IPInfo = new HttpClient()
{
    BaseAddress = new Uri("https://ipinfo.io")
};

var Response_IPInfo = await Client_IPInfo.GetStringAsync("json");

using JsonDocument JSON_IPInfo = JsonDocument.Parse("[" + Response_IPInfo + "]");
JsonElement Elements_IPInfo = JSON_IPInfo.RootElement;

var Record_IPInfo = Elements_IPInfo[0];
Data = Record_IPInfo.GetProperty("ip").ToString();
Console.WriteLine(Data);
Console.WriteLine();


var New = new Record(Data, Name, TTL, Type);
Console.WriteLine(New.data);
Console.WriteLine(New.name);
Console.WriteLine(New.ttl);
Console.WriteLine(New.type);
Console.WriteLine();

var JSON_String = JsonSerializer.Serialize(New);

await Client_GoDaddy.PutAsync(Method, new StringContent("[" + JSON_String + "]", Encoding.UTF8, "application/json"));


public class Record
{
    public String data { get; set; }
    public String name { get; set; }
    public int ttl { get; set; }
    public String type { get; set; }

    public Record(String Data, String Name, int TTL, String Type)
    {
        this.data = Data;
        this.name = Name;
        this.ttl = TTL;
        this.type = Type;
    }
}
