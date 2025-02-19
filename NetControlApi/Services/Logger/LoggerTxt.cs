namespace NetControlApi.Services.Logger;

public class LoggerTxt : ILogger
{
    private string _filePath;

    public LoggerTxt()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "log.txt");
    }

    public async Task Write(string log)
    {
        await File.AppendAllTextAsync(_filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {log}{Environment.NewLine}");
    }
}
