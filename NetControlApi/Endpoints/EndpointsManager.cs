using System.Diagnostics;

namespace NetControlApi.Endpoints;

public static class EndpointsManager
{
    public static void RoutingEndpoints(this WebApplication app)
    {
        var path = @"C:\Users\Enoonni\AspLess\NetControl\DataServer\bin\Debug\net8.0\DataServer.exe";

        Process? dataServerProcess = null;

        bool IsServerRunning() => dataServerProcess != null && !dataServerProcess.HasExited;

        app.MapGet("/", () => 
        {
            bool isRunning = IsServerRunning();
            
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
            if(dataServerProcess == null || dataServerProcess.HasExited)
            {
                try
                {
                    dataServerProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = false
                        }
                    };
                    dataServerProcess.Start();
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
            if(dataServerProcess != null && !dataServerProcess.HasExited)
            {
                try
                {
                    dataServerProcess.Kill();
                    dataServerProcess.WaitForExit();
                    dataServerProcess.Dispose();
                    dataServerProcess = null;
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
