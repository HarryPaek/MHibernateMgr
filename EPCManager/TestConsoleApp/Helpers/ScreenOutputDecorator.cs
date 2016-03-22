using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class ScreenOutputDecorator
    {
        public static void PrintHeader(string labelText)
        {
            PrintDecoration(labelText);
        }

        public static void PrintFooter()
        {
            PrintFooter(string.Empty);
        }

        public static void PrintFooter(string labelText)
        {
            PrintDecoration(labelText);
            Console.WriteLine();
        }

        public static void PrintSubListHeader(string labelText)
        {
            PrintSubListDecoration(labelText);
        }

        private static void PrintDecoration(string labelText)
        {
            Console.WriteLine("===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====");
            if(!string.IsNullOrWhiteSpace(labelText))
                Console.WriteLine(string.Format("===== ===== ===== ===== ===== ===== ===== ===== ===== {0} ===== ===== ===== ===== ===== ===== ===== ===== =====", labelText));
            Console.WriteLine("===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====");
        }

        private static void PrintSubListDecoration(string labelText)
        {
            Console.WriteLine("%%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%%");
            if (!string.IsNullOrWhiteSpace(labelText))
                Console.WriteLine(string.Format("%%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% {0} %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%%", labelText));
            Console.WriteLine("%%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%% %%%%%");
        }
    }
}
