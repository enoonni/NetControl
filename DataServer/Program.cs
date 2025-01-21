using DataServer.Core;

TcpServer server = new TcpServer(1337);
server.Start();

while (true)
{
    try    
    {
        var input = Console.ReadLine();

        switch (input)
        {
            case "0":
                server.Stop();
                Environment.Exit(0);
                break;

            default:
                break;
        }        
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        await Task.Delay(1000);
    }
}