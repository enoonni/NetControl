using System.Diagnostics;

namespace NetControlApi.Services.TcpServer;

public class TcpServerManager : IDisposable, ITcpServerManager
{
    private readonly string _path;
    private Process? _dataServerProcess;
    public bool IsServerRunning => _dataServerProcess is {HasExited: false};

    public TcpServerManager(string path)
    {
        _path = path;
    }

    public bool Start()
    {
        if(IsServerRunning)
        {
            return false;
        }

        _dataServerProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _path.ToString(),
                UseShellExecute = false
            }
        };
        _dataServerProcess.Start();
        return true;       
    }

    public bool Stop()
    {
        if(!IsServerRunning)
        {
            return false;
        }

        _dataServerProcess!.Kill();
        _dataServerProcess.WaitForExit();
        _dataServerProcess.Dispose();
        _dataServerProcess = null;
        return true;
    }

    public void Dispose()
    {
        if (_dataServerProcess != null)
        {
            Stop();
        }
    }
}
