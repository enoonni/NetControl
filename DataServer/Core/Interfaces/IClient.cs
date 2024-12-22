namespace DataServer.Core.Interfaces;

public interface IClient
{
    Task SendDataAsync(byte[] data);
    void Disconnect();
}
