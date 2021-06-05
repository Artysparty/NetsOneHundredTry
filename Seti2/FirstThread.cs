﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Seti2;
using System.IO;

namespace Seti2
{
    public class FirstThread
    {
        private Semaphore _sendSemaphore;
        private Semaphore _receiveSemaphore;

        private BitArray[] _receivedMessages;

        private PostToSecondWT _post;

        public FirstThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
        }

        public void FirstThreadMain(object obj)
        {
            _post = (PostToSecondWT)obj;

            ConsoleHelper.WriteToConsole("1 поток", "Начинаю работу.");

            // TODO: build frames

            var connection = new Frame();

            connection.Control = new BitArray(16);
            connection.Control.Write(0, Utils.DecimalToBinary(200));
            connection.Checksum = Utils.DecimalToBinary(0);
            connection.Data = new BitArray(16);

            _post(new[] { connection.ToBitArray() });

            ConsoleHelper.WriteToConsole("1 поток", "Отправлен запрос на подключение");

            _sendSemaphore.Release();
            _receiveSemaphore.WaitOne();

            ConsoleHelper.WriteToConsole("1 поток", "Получаю ответ");

            // TODO: Check all received messages

            var connectionFrame = Frame.Parse(_receivedMessages[0]);

            byte[] controlBytes = new byte[2];
            connectionFrame.Control.CopyTo(controlBytes, 0);

            if (controlBytes[0] == 201)
            {
                ConsoleHelper.WriteToConsole("1 поток", "Подключение разрешено");
            }
            else if (controlBytes[0] == 202)
            {
                ConsoleHelper.WriteToConsole("1 поток", "Подключение запрещено. Заканчиваю работу.");
                System.Environment.Exit(-1);
            }

            var fileBytes = File.ReadAllBytes("C:\\Users\\artem\\Dropbox\\Мой ПК (LAPTOP-V6M1QK29)\\Desktop\\text.txt");

            var split = Utils.SplitFile(fileBytes);
            //отправка файла
            foreach (var s in split)
            {
                Frame frame = new Frame();

                frame.Control = new BitArray(16);

                frame.Data = new BitArray(s);

                frame.Checksum = Utils.DecimalToBinary(Utils.CheckSum(frame.Data));

                var frameBitArr = frame.ToBitArray();
                _post(new[] { frameBitArr });
                _sendSemaphore.Release();

                _receiveSemaphore.WaitOne();

                ConsoleHelper.WriteToConsole("1 поток", "Ожидаю квитанцию.");

                var receipt = Frame.Parse(_receivedMessages[0]);

                byte[] control = new byte[2];
                receipt.Control.CopyTo(control, 0);

                if (control[0] == 31)
                {
                    ConsoleHelper.WriteToConsole("1 поток", "Квитанция true получена.");
                }
                else if (control[0] == 32)
                {
                    ConsoleHelper.WriteToConsole("1 поток", "Квитанция false получена.");
                }
            }

            var endFrame = new Frame();

            endFrame.Control = new BitArray(16);
            endFrame.Control.Write(0, Utils.DecimalToBinary(90));
            endFrame.Checksum = Utils.DecimalToBinary(0);
            endFrame.Data = new BitArray(16);

            _post(new[] { endFrame.ToBitArray() });

            _sendSemaphore.Release();

            _receiveSemaphore.WaitOne();

            // BitArray[] bitArrays = new BitArray[5];

            // for (int i = 0; i < 5; i++)
            // {
            //     var message = Frame.CreateFrameBitArray();
            //     bitArrays[i] = message;
            // }

            // _post(bitArrays);
            // _sendSemaphore.Release();

            // _receiveSemaphore.WaitOne();

            // ConsoleHelper.WriteToConsole("1 поток", "Ожидаю квитанцию.");

            // var receipt = Frame.Parse(_receivedMessages[0]);

            // byte[] control = new byte[2];
            // receipt.Control.CopyTo(control, 0);

            // if (control[0] == 31)
            // {
            //     ConsoleHelper.WriteToConsole("1 поток", "Квитанция true получена.");
            // }
            // else if (control[0] == 32)
            // {
            //     ConsoleHelper.WriteToConsole("1 поток", "Квитанция false получена.");
            // }
        }

        public void ReceiveData(BitArray[] arrays)
        {
            _receivedMessages = arrays;
        }
    }
}