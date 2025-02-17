using System.Text;

namespace NetControlApi.Services.TcpServer;

public class TcpServerManager
{
    private StringBuilder _path;

    public TcpServerManager(string path)
    {
        _path = new StringBuilder(path);
    }

    public bool Start()
    {
        return false;
    }

    public bool Stop()
    {
        return false;
    }
}
