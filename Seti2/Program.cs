using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Seti2;

namespace Seti2
{
    public delegate void PostToFirstWT(BitArray[] message);
    public delegate void PostToSecondWT(BitArray[] message);
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.WriteToConsole("Главный поток", "");

            Semaphore firstReceiveSemaphore = new Semaphore(0, 1);
            Semaphore secondReceiveSemaphore = new Semaphore(0, 1);

            FirstThread firstThread = new FirstThread(ref secondReceiveSemaphore, ref firstReceiveSemaphore);
            SecondThread secondThread = new SecondThread(ref firstReceiveSemaphore, ref secondReceiveSemaphore);

            Thread threadFirst = new Thread(new ParameterizedThreadStart(firstThread.FirstThreadMain));
            Thread threadSecond = new Thread(new ParameterizedThreadStart(secondThread.SecondThreadMain));

            PostToFirstWT postToFirstWt = new PostToFirstWT(firstThread.ReceiveData);
            PostToSecondWT postToSecondWt = new PostToSecondWT(secondThread.ReceiveData);

            threadFirst.Start(postToSecondWt);
            threadSecond.Start(postToFirstWt);

            Console.ReadLine();
        }
    }

}