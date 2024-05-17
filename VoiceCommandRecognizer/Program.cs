using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VoiceCommandRecognizer
{
    internal class Program
    {
        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static ConsoleEventDelegate handler;
        public static VCWebSocketServer VCWebSocketServer;
        static Process myProcess;

        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            using (VCWebSocketServer = new VCWebSocketServer())
            {
                //VCWebSocketServer.HandleMessage(new VoiceCommandMessage() { type = VCType.ADD, lsCommands = new List<string> { "Test" } });
                while (true)
                {
                    //...
                }
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                myProcess?.CloseMainWindow();
                VCWebSocketServer?.CloseSocket();
            }
            Log("ConsoleEventCallback", MsgType.INFO);
            return false;
        }

        internal static void Log(string msg, MsgType lvl)
        {
#if !DEBUG
            if (lvl == MsgType.DEBUG) return;
#endif
            switch (lvl)
            {
                case MsgType.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case MsgType.SUCCESS:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case MsgType.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MsgType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MsgType.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                default:
                    break;
            }
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss:FF")}] | {msg}");
            Console.ResetColor();
        }

        internal enum MsgType
        {
            INFO,
            SUCCESS,
            WARNING,
            ERROR,
            DEBUG
        }
    }
}
