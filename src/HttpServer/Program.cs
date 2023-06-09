// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

var server = new HttpListener();
try
{
    var config = await ReadConfigAsync();
    server.Prefixes.Add(config.Prefix);
    foreach (var item in config.EndPoints)
    {
        System.Console.WriteLine(item);
    }
    server.Start();// start server
    Info("server started");
    while (true)
    {
        HttpListenerContext context = await server.GetContextAsync();
        var request = context.Request;
        var response = context.Response;
        //var user = context.User;

        var data = await SelectPageAsync(request, config.EndPoints.ToArray());

        DisplayRequiestInfo(request);
        await SendResponseAsync(response, data);
    }
}
catch (System.Exception ex)
{
    System.Console.WriteLine(ex.Message);
}
finally
{
    server.Stop(); // stop server
    server.Close();// closing HttpListener
}


async Task<byte[]> SelectPageAsync(HttpListenerRequest request, EndPoint[] endpoints)
{
    var endpoint = request.RawUrl;
    Info("trying to select page with endpoint: " + endpoint);
    foreach (var ep in endpoints.Where(ep => endpoint == ep.Url || endpoint == ep.Url + '/'))
    {
        return await File.ReadAllBytesAsync(ep.Page);
    }
    return await GetHomePageAsync();
}

async Task SendResponseAsync(HttpListenerResponse response, byte[] buffer)
{
    Info("sending response..");

    //var buffer = Encoding.UTF8.GetBytes(data);
    response.ContentLength64 = buffer.Length;
    using var output = response.OutputStream;
    await output.WriteAsync(buffer);
    await output.FlushAsync();
    Info("done.");
}

async Task<byte[]> GetHomePageAsync()
{
    return await File.ReadAllBytesAsync("index.html");
}

void DisplayRequiestInfo(HttpListenerRequest request)
{
    Info("request info:");
    Console.WriteLine($"app address: {request.LocalEndPoint}");
    Console.WriteLine($"client address: {request.RemoteEndPoint}");
    Console.WriteLine(request.RawUrl);
    Console.WriteLine($"requested address: {request.Url}");
    Console.WriteLine($"headers: ");
    foreach(string? header in request.Headers.Keys)
        Console.WriteLine($"\t{header}:{request.Headers[header]}");
}

void Info(string msg) => System.Console.WriteLine("[INFO] " + msg);

async Task<Config> ReadConfigAsync()
{
    var json = await File.ReadAllTextAsync("config.json");
    var config = JsonConvert.DeserializeObject<Config>(json);
    return config;
}

readonly record struct EndPoint(string Url, string Page);
readonly record struct Config(string Prefix, List<EndPoint> EndPoints);
