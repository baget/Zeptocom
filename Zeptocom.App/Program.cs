using CommandLine;
using System;
using System.IO.Ports;
using System.Reflection;
using System.Threading;

namespace Zeptocom.App
{
    class Program
    {
        static readonly string AppTitle = $"Zeptocom v{Assembly.GetEntryAssembly().GetName().Version}";
        static TerminalScreen screen = new TerminalScreen(AppTitle);

        public class Options
        {
            [Option('b', "baudrate", Required = false, HelpText = "Set baudrate (default is 115200)", Default = 115200)]
            public int BaudRate { get; set; }

            [Value(0, MetaName = "port", HelpText = "COM Port name (e.g. COM1), default is COM1", Default = "COM1")]
            public string Port { get; set; }
        }

        static void Main(string[] args)
        {
            TerminalPort.PortSettings portSettings = null;

            portSettings = HandleArgs(args, portSettings);

            TerminalPort terminalPort = null;

            try
            {
                terminalPort = new TerminalPort(portSettings, screen);

                terminalPort.Start();
            }
            catch (Exception ex)
            {
                Console.Clear();
                ExitWithError(ex.Message);
            }


            bool _stop = false;

            while (!_stop)
            {
                var key = Console.ReadKey(false);

                if (key.Key == ConsoleKey.Escape)
                {
                    terminalPort.Stop();
                    _stop = true;
                    break;
                }
                else if (key.Key == ConsoleKey.F1)
                {
                    terminalPort.Stop();

                    PrintAbout(portSettings);

                    screen.Reset();
                    terminalPort.Start();

                }
                else if (key.Key == ConsoleKey.F2)
                {
                    screen.Reset();
                }
                else
                {
                    terminalPort.Write(key.KeyChar.ToString());
                }
            }

        }

        private static void PrintAbout(TerminalPort.PortSettings portSettings)
        {
            screen.Reset();
            Console.WriteLine(AppTitle);
            Console.WriteLine("Copyright (C) Oren Weil, 2021\n");
            Console.WriteLine($"{portSettings}\n");
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static TerminalPort.PortSettings HandleArgs(string[] args, TerminalPort.PortSettings portSettings)
        {
            Parser.Default.ParseArguments<Options>(args)
                          .WithParsed<Options>(o =>
                          {
                              var ports = SerialPort.GetPortNames();

                              var foundStr = Array.Find(ports, (string s) =>
                              {
                                  return o.Port == s;
                              });

                              if (String.IsNullOrEmpty(foundStr))
                              {
                                  ExitWithError("Invalid COM Port name or port does not exists");
                              }

                              portSettings = new TerminalPort.PortSettings(o.Port, o.BaudRate);
                          });
            return portSettings;
        }

        private static void ExitWithError(string msg)
        {
            Console.Error.WriteLine($"Error: {msg}");
            Environment.Exit(1);
        }
    }
}
