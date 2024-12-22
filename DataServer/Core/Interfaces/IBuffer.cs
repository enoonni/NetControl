namespace DataServer.Core.Interfaces;

public interface IDataBuffer
{
    Task PutDataAsync(byte[] data);
    Task<byte[]> GetDataAsync();
}
