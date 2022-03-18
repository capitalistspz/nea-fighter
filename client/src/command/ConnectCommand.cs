using System;
using System.Net;
using client.core;
using common.command;

namespace client.command
{
    public class ConnectCommand
    {
        public static void Register()
        {
            var rootNode = new CommandNode("connect", CommandInputType.Literal)
                .Next(new CommandNode("ip", CommandInputType.Argument)
                    .Next(new CommandNode("port", CommandInputType.Argument)
                        .Run(cmd =>
                        {
                            var ipString = cmd.GetArg("ip");
                            var portString = cmd.GetArg("port");
                            var game = (ClientGame) cmd.GetGame();
                            game.Connect(IPAddress.Parse(ipString), Int32.Parse(portString), String.Empty);
                        })
                    ));
            Commands.AddCommand(rootNode, "Connect with ipv4 and port by typing\n" +
                                          "/connect ip:<ipaddress> port:<port>\n" +
                                          "Example: /connect ip:127.0.0.1 port:12345");
        }
    }
}