using NetControlApi.Endpoints;
using NetControlApi.Services.TcpServer;

var builder = WebApplication.CreateBuilder(args);
var path = builder.Configuration["DataServer:ExecutablePath"];
if(path == null)
{
    throw new Exception("Path is null");
}

var tcpServer = new TcpServerManager(path);

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.RoutingEndpoints(tcpServer);

app.Run();