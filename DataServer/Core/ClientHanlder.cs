using System.Text;
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

    public void Start()
    {
        Task.Run(async () =>
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
        });
    }

    public async Task HandlingData(ClientHandlerBufferData data)
    {
        // await data.Client.SendDataAsync(data.Data);
        try
        {
            Console.WriteLine(BitConverter.ToString(data.Data));
            await data.Client.SendDataAsync(Encoding.UTF8.GetBytes("Accept"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Stop()
    {
        _cts.Cancel();
    }
}
