namespace NetControlApi.Services.Logger;

public interface ILogger
{
    Task Write(string log);
}
