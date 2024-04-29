using PvPGameServer;

var serverOption = ParseCommandLine(args);

if(serverOption == null)
{
    return;
}

MainServer ServerApp = new MainServer();
ServerApp.InitConfig(serverOption);
ServerApp.CreateAndStartServer();


while (true)
{
    System.Threading.Thread.Sleep(50);

    if (Console.KeyAvailable)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        if(key.KeyChar == 'q')
        {
            Console.WriteLine("Server Terminate...");
            ServerApp.StopServer();
            break;
        }

    }
}


static PvPServerOption ParseCommandLine(string[] args)
{
    var res = CommandLine.Parser.Default.ParseArguments<PvPServerOption>(args)
        as CommandLine.Parsed<PvPServerOption>;

    if(res == null)
    {
        return null;
    }

    return res.Value;

}