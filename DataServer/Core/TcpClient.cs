using System.Net.Sockets;
using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class TcpClient : IClient
{
    private readonly Socket _socket;
    private readonly CancellationTokenSource _cts;
    private readonly IBuffer<byte[]> _sendBuffer = new DataBuffer();
    private readonly IBuffer<ClientHandlerBufferData> _clientHandlerBuffer;
    private readonly string _address;

    public TcpClient(Socket socket, IBuffer<ClientHandlerBufferData> clientHandlerBuffer, string address)
    {
        _socket = socket;
        _cts = new();
        _clientHandlerBuffer = clientHandlerBuffer;
        _address = address;
    }

    public bool IsConnected => _socket.Connected;

    public void Open()
    {
        StartReceiving();
        StartSending();        
    }

    public async Task SendDataAsync(byte[] data)
    {
        await _sendBuffer.PutDataAsync(data);
    }

    public void Disconnect()
    {
        _cts.Cancel();
    }

    private void StartReceiving()
    {
        Task.Run(async() =>
        {
            byte[] buffer = new byte[2048];

            while(!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var size = await _socket.ReceiveAsync(buffer, _cts.Token);
                    if(size == 0)
                    {
                        _cts.Cancel();
                        throw new Exception("Client connection lost");
                    }
                    var data = new byte[size];
                    Array.Copy(buffer, 0, data, 0, size);
                    await _clientHandlerBuffer.PutDataAsync(new ClientHandlerBufferData(this, data));
                }
                catch (OperationCanceledException)
                {
                    
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    if(_socket.Connected)
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                    }
                    _socket.Dispose();
                    _cts.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            await Task.Delay(1000);
            _cts.Dispose();
        });    
    }

    private void StartSending()
    {
        Task.Run(async () =>
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
    }
}
