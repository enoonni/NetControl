using System;
using DataServer.Core.Interfaces;

namespace DataServer.Core;

public class ClientHandlerBuffer<ClientHandlerBufferData> : IBuffer<ClientHandlerBufferData>
{
    private readonly List<ClientHandlerBufferData> _buffer;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxBufferSize;

    public ClientHandlerBuffer(int maxBufferSize = 1000)
    {
        _buffer = new List<ClientHandlerBufferData>();
        _semaphore = new SemaphoreSlim(1);
        _maxBufferSize = maxBufferSize;
    }

    public bool IsEmpty => _buffer.Count == 0;
    public async Task PutDataAsync(ClientHandlerBufferData data)
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

    public async Task<ClientHandlerBufferData> GetDataAsync()
    {
        await _semaphore.WaitAsync();
        if(_buffer.Count > 0)
        {
            var data = _buffer[0];
            _buffer.RemoveAt(0);
            _semaphore.Release();

            return data;
        }
        else
        {
            throw new InvalidOperationException("ClientHandlerBuffer is Empty!");
        }
    }
}
