namespace DataServer.Core;

public record ClientHandlerBufferData(TcpClient Client, byte[] Data);
