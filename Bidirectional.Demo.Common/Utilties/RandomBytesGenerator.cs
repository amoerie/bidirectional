using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Utilties
{
    public static class RandomBytesGenerator
    {
        public static byte[] Generate(int numberOfBytes)
        {
            var bytes = new byte[numberOfBytes];

            var random = new Random();
            for (int i = 0; i < numberOfBytes; i++)
                bytes[i] = (byte)random.Next(256);

            return bytes;
        }
    }
}
