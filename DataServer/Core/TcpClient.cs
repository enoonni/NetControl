using System.Net.Sockets;
using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class TcpClient : IClient
{
    private readonly Socket _socket;
    private readonly CancellationTokenSource _cts;
    private readonly IBuffer<byte[]> _sendBuffer;
    private readonly ClientHandlerBuffer<ClientHandlerBufferData> _clientHandlerBuffer;

    public TcpClient(Socket socket, IBuffer<byte[]> sendBuffer, ClientHandlerBuffer<ClientHandlerBufferData> clientHandlerBuffer)
    {
        _socket = socket;
        _cts = new();
        _sendBuffer = sendBuffer;
        _clientHandlerBuffer = clientHandlerBuffer;
    }

    public async Task StartAsync()
    {
        byte[] buffer = new byte[2048];

        var taskSending = Task.Run(async () =>
        {
            while(!_cts.Token.IsCancellationRequested)
            {
                if(!_sendBuffer.IsEmpty)
                {
                    try
                    {
                        var data = await _sendBuffer.GetDataAsync();
                        await _socket.SendAsync(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }                    
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        });

        while(!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var size = await _socket.ReceiveAsync(buffer, _cts.Token);
                var data = new byte[size];
                Array.Copy(buffer, 0, data, 0, size);
                await _clientHandlerBuffer.PutDataAsync(new ClientHandlerBufferData(this, data));
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
