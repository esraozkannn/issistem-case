using System;

namespace VakaCalismasi
{
    public class ConsoleReadLine : IConsoleReadLine
    {
        public virtual string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
