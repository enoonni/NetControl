using System.Net;
using System.Net.Sockets;

namespace DataServer.Core;

public class TcpServer
{
    private readonly int _port;
    private readonly Socket _mainSocket;
    private readonly IPEndPoint _ipEndPoint;
    private readonly CancellationTokenSource _mainCts;

    public TcpServer(int port)
    {
        _mainCts = new();
        _port = port;
        _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
        _mainSocket.Bind(_ipEndPoint);
    }

    public async Task StartAsync()
    {
        _mainSocket.Listen(10);
        while(!_mainCts.Token.IsCancellationRequested)
        {
            try
            {
                var clientSocket = await _mainSocket.AcceptAsync(_mainSocket, _mainCts.Token);

            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}
