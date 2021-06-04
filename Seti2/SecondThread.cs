using System;
using System.Collections;
using System.Threading;
using Seti2;

namespace Seti2
{
    public class SecondThread
    {
        private Semaphore _sendSemaphore;
        private Semaphore _receiveSemaphore;

        private BitArray[] _receivedMessages;
        private PostToFirstWT _post;

        public SecondThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
        }

        public void SecondThreadMain(Object obj)
        {
            _post = (PostToFirstWT)obj;
            ConsoleHelper.WriteToConsole("2 поток", "Начинаю работу.");

            _receiveSemaphore.WaitOne();

            var frame = Frame.Parse(_receivedMessages[0]);

            byte[] controlBytes = new byte[2];
            frame.Control.CopyTo(controlBytes, 0);

            //200 - запрос на подключение, ответ 201, 
            //иначе ответ 202, программа заканчивает работу
            //30 - кадр с данными
            //31 - квитанция о получении
            //32 - квитанция об отсутствии или повреждении данных

            if (controlBytes[0] == 200)
            {
                Frame resp = Utils.CreateTrueConnection();
                _post(new[] { resp.ToBitArray() });
                _sendSemaphore.Release();
            }

            _receiveSemaphore.WaitOne();
            ConsoleHelper.WriteToConsole("2 поток", "Ожидаю кадр.");

            for (int i = 0; i < _receivedMessages.Length; i++)
            {
                ConsoleHelper.WriteToConsoleArray("Кадр", _receivedMessages[i]);
            }

            var response = new Frame();

            response.Control = new BitArray(16);
            response.Control.Write(0, Utils.DecimalToBinary(32));
            response.Checksum = Utils.DecimalToBinary(0);
            response.Data = new BitArray(16);

            _post(new[] { response.ToBitArray() });
            _sendSemaphore.Release();
        }

        public void ReceiveData(BitArray[] arrays)
        {
            _receivedMessages = arrays;
        }
    }
}