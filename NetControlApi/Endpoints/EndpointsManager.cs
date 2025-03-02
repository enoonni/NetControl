using NetControlApi.Services.TcpServer;

namespace NetControlApi.Endpoints;

public static class EndpointsManager
{
    public static void RoutingEndpoints(this WebApplication app, ITcpServerManager tcpServer)
    {
        app.MapGet("/", () => 
        {
            bool isRunning = tcpServer.IsServerRunning;
            
            return Results.Content($@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Управление Console App</title>
                </head>
                <body>
                    <h1> GGWP </h1><br>
                    <h2>Status: {(isRunning ? "Включен" : "Выключен")}</h2><br>

                    <form action='/start' method='post'>
                        <button type='submit' {(isRunning ? "disabled" : "enabled")}>Server Start</button>
                    </form>

                    <form action='/stop' method='post'>
                        <button type='submit' {(!isRunning ? "disabled" : "")}>Server Stop</button>
                    </form>
                </body>
                </html>", "text/html");
        });

        app.MapPost("/start", () =>
        {
            if(!tcpServer.IsServerRunning)
            {
                try
                {
                    tcpServer.Start();
                }

                catch (Exception ex)
                {
                    // Обработка ошибок. Логирование крайне рекомендуется!
                    Console.WriteLine($"Ошибка запуска: {ex.Message}");
                    return Results.BadRequest($"Ошибка запуска: {ex.Message}"); // Возвращаем ошибку клиенту
                }

                return Results.Ok("Сервер запущен.");
            }
            else
            {
                return Results.BadRequest("Сервер уже запущен.");
            }
        });

        app.MapPost("/stop", () =>
        {
            if(tcpServer.IsServerRunning)
            {
                try
                {
                    tcpServer.Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка остановки: {ex.Message}");
                    return Results.BadRequest($"Ошибка остановки: {ex.Message}");
                }

                return Results.Ok("Сервер остановлен.");
            }
            else
            {
                return Results.BadRequest("Сервер не запущен.");
            }
        });
    }
}
