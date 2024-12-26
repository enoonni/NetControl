using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class ClientHanlder
{
    private readonly IBuffer<ClientHandlerBufferData> _buffer;
    public IBuffer<ClientHandlerBufferData> Buffer => _buffer;
    
    private readonly CancellationTokenSource _cts;

    public ClientHanlder()
    {
        _buffer = new ClientHandlerBuffer<ClientHandlerBufferData>();
        _cts = new();
    }

    public async Task Start()
    {
        while(!_cts.Token.IsCancellationRequested)
        {
            if(!_buffer.IsEmpty)
            {
                try
                {
                    await HandlingData(await _buffer.GetDataAsync());
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                await Task.Delay(100);
            }
        }
        _cts.Dispose();
    }

    public async Task HandlingData(ClientHandlerBufferData data)
    {
        await data.Client.SendDataAsync(data.Data);
    }
}
