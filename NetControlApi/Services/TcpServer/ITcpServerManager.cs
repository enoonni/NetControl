namespace NetControlApi.Services.TcpServer;

public interface ITcpServerManager
{
    bool IsServerRunning{ get; }
    bool Start();
    bool Stop();
}
