using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zeptocom.App
{
    public class TerminalScreen
    {
        readonly static MenuItem[] _items = { new MenuItem("Exit",   "ESC"), 
                                             new MenuItem("About",   "F1"),
                                             new MenuItem("Clear", "F2"),};

        private string _title;
        public TerminalScreen(string title) 
        {
            _title = title;

            Reset();

        }

        public void Reset()
        {
            lock (this)
            {
                Console.ResetColor();
                Console.Clear();

                DrawTitle(_title);
                DrawMenu(_items);

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 1);
            }
        }

        private static void DrawTitle(string title)
        {
            var midScreenPoint = Console.WindowWidth / 2;
            Console.SetCursorPosition(midScreenPoint - (title.Length / 2), 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(title);
            Console.ResetColor();
        }
        
        public record MenuItem(string Name, string Hotkey);
        private static void DrawMenu(MenuItem[] items)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            foreach (var item in items)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write(item.Hotkey);            
                Console.Write(" ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write(item.Name);

                Console.ResetColor();
                Console.Write("  ");
            }
        }

        public void WriteLine(string str = "")
        {
            this.Write(str + "\r\n");
        }        
        public void Write(string str = "")
        {
            lock (this)
            {
                int startInx = 0;
                string subStr;

                var endInx = str.IndexOf('\r', startInx);

                while (endInx != -1)
                {
                    ScrollBuffer();

                    subStr = str.Substring(startInx, endInx - startInx);
                    Console.WriteLine(subStr);
                    startInx = endInx + 2;
                    if (startInx > str.Length)
                        break;

                    endInx = str.IndexOf('\r', startInx);



                    Thread.Sleep(10);
                }

                if (startInx < str.Length)
                {
                    ScrollBuffer();

                    subStr = str.Substring(startInx);
                    Console.Write(subStr);
                }

            }        }

        private static void ScrollBuffer()
        {
            // Check if lines reached Menu Line or not
            if (Console.GetCursorPosition().Top == Console.WindowHeight - 1)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                // Move Buffer one line above
                Console.MoveBufferArea(0, 2, Console.WindowWidth, Console.WindowHeight - 3, 0, 1);
#pragma warning restore CA1416 // Validate platform compatibility
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
            }
        }
    }
}
