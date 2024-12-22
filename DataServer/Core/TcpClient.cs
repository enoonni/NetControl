using System;
using System.Net.Sockets;
using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class TcpClient : IClient
{
    private readonly Socket _socket;
    private readonly CancellationTokenSource _cts;
    private readonly IDataBuffer _sendBuffer;

    public TcpClient(Socket socket)
    {
        _socket = socket;
        _cts = new();
    }

    public async Task StartAsync()
    {
        byte[] buffer = new byte[2048];
        while(!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var size = await _socket.ReceiveAsync(buffer, _cts.Token);
                var data = new byte[size];
                Array.Copy(buffer, 0, data, 0, size);
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        _cts.Dispose();
    }

    public async Task SendDataAsync(byte[] data)
    {
        await _sendBuffer.PutDataAsync(data);
    }

    public void Disconnect()
    {
        _cts.Cancel();
    }
}
