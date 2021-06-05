using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Seti2
{
    public class Frame
    {
        static Random rand = new Random();

        private const int ControlSize = 16;

        private const int ChecksumSize = 8;

        public BitArray Control { get; set; }

        public BitArray Data { get; set; }

        public BitArray Checksum { get; set; }

        public BitArray ToBitArray()
        {
            BitArray result = new BitArray(ControlSize + ChecksumSize + Data.Count);

            result.Write(0, Control);
            result.Write(ControlSize, Data);
            result.Write(ControlSize + Data.Count, Checksum);

            return result;
        }

        public static Frame Parse(BitArray bitArray)
        {
            Frame frame = new Frame();

            frame.Control = bitArray.Subsequence(0, ControlSize);
            frame.Checksum = bitArray.Subsequence(bitArray.Length - ChecksumSize, ChecksumSize);
            frame.Data = bitArray.Subsequence(ControlSize, bitArray.Length - ControlSize - ChecksumSize);

            return frame;
        }

        public static BitArray CreateFrameBitArray()
        {

            Frame frame = new Frame();

            frame.Control = new BitArray(ControlSize);

            frame.Data = new BitArray(Utils.DecimalToBinary(rand.Next(10000)));

            frame.Checksum = Utils.DecimalToBinary(Utils.CheckSum(frame.Data));


            return frame.ToBitArray();
        }
    }
}