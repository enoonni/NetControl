using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class DataBuffer : IBuffer<byte[]>
{
    private readonly List<byte[]> _buffer;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxBufferSize;

    public DataBuffer(int maxBufferSize = 1000)
    {
        _buffer = new List<byte[]>();
        _semaphore = new SemaphoreSlim(1);
        _maxBufferSize = maxBufferSize;
    }

    public bool IsEmpty => _buffer.Count == 0;
    public async Task PutDataAsync(byte[] data)
    {
        await _semaphore.WaitAsync();
        {
            if(_buffer.Count < _maxBufferSize)
            {
                _buffer.Add(data);
            }
        }
        _semaphore.Release();
    }

    public async Task<byte[]> GetDataAsync()
    {
        var data = new byte[0];
        await _semaphore.WaitAsync();
        if(_buffer.Count > 0)
        {
            data = _buffer[0];
            _buffer.RemoveAt(0);
        }
        _semaphore.Release();

        return data;
    }
}
