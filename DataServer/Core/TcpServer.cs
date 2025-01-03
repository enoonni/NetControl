using System.Net;
using System.Net.Sockets;

namespace DataServer.Core;

public class TcpServer
{
    private readonly int _port;
    private readonly Socket _mainSocket;
    private readonly IPEndPoint _ipEndPoint;
    private readonly CancellationTokenSource _mainCts;
    private readonly ClientHanlder _clientHanlder;

    public TcpServer(int port)
    {
        _mainCts = new();
        _port = port;
        _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
        _mainSocket.Bind(_ipEndPoint);
        _clientHanlder = new();
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            _mainSocket.Listen(10);
            while(!_mainCts.Token.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _mainSocket.AcceptAsync(_mainCts.Token);
                    var client = new TcpClient(clientSocket, _clientHanlder.Buffer);
                    if(clientSocket.RemoteEndPoint is IPEndPoint clientEndPoint)
                    {
                        Console.WriteLine($"Connect: {clientEndPoint}");
                    }
                    try
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Token is canceled");
                }
            }
            _mainCts.Dispose();
        });
    }

    public void Stop()
    {
        _mainCts.Cancel();        
    }
}
