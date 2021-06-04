using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seti2
{
    public static class ConsoleHelper
    {
        public static object LockObject = new Object();
        public static void WriteToConsole(string info, string write)
        {
            lock (LockObject)
            {
                Console.WriteLine(info + " : " + write);
            }

        }
        public static void WriteToConsoleArray(string info, BitArray array)
        {
            lock (LockObject)
            {
                Console.Write(info + " : ");
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");

                }
                Console.WriteLine();
            }
        }

        public static void WriteBitArray(BitArray array)
        {
            lock (LockObject)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == true)
                        Console.Write("1");
                    else
                        Console.Write("0");
                }
            }
        }
    }
}