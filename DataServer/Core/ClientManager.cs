namespace DataServer.Core;

public class ClientManager
{
    private readonly List<TcpClient> _clients;
    private readonly CancellationTokenSource _cts;
    private readonly SemaphoreSlim _semaphore;

    public ClientManager()
    {
        _cts = new();
        _semaphore = new SemaphoreSlim(1);
        _clients = new List<TcpClient>();
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            while(!_cts.Token.IsCancellationRequested)
            {
                if(_clients.Count > 0)
                {
                    foreach(var client in _clients)
                    {
                        if(!client.IsConnected)
                        {
                            _clients.RemoveAt(_clients.IndexOf(client));
                        }
                    }
                    await Task.Delay(1000);
                }
                else
                {
                    await Task.Delay(1000);
                }
                
            }
            
            await Task.Delay(100);
            _cts.Dispose();
            _semaphore.Dispose();
        });
    }

    public async Task AddClientAsync(TcpClient client)
    {
        await _semaphore.WaitAsync();
        _clients.Add(client);
        _semaphore.Release();
    }

    public void Stop()
    {
        _cts.Cancel();
    }
}
