using System;
using Serilog;

namespace server
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().WriteTo
                .File("logs/client.log", rollingInterval: RollingInterval.Hour).CreateLogger();
            using (var game = new ServerGame())
                game.Run();
        }
    }
}