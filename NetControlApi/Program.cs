using NetControlApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.RoutingEndpoints();


app.Run();
