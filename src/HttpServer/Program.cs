// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;

var server = new HttpListener();
server.Prefixes.Add("http://localhost:1337/home/");
server.Start();// start server
Info("server started");
while (true)
{
    HttpListenerContext context = await server.GetContextAsync();
    var request = context.Request;
    var response = context.Response;
    var user = context.User;
    DisplayRequiestInfo(request);
    await SendResponseAsync(response);
}
server.Stop(); // stop server
server.Close();// closing HttpListener

async Task SendResponseAsync(HttpListenerResponse response)
{
    Info("sending response..");
    var data = await GetHomePageAsync();

    //var buffer = Encoding.UTF8.GetBytes(data);
    var buffer = data;
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
