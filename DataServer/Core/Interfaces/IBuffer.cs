namespace DataServer.Core.Interfaces;

public interface IBuffer<T>
{
    bool IsEmpty { get; }
    Task PutDataAsync(T data);
    Task<T> GetDataAsync();
}
