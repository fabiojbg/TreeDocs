using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Apps.Sdk.Helpers
{
    public static class Compressor
    {
        //---------------------------------------------------------------------------------------------------------------------------------
        public static string CompressToString(string input)
        {
            return Convert.ToBase64String(CompressToByteArray(Encoding.UTF8.GetBytes(input)));
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public static string CompressToString(byte[] input)
        {
            return Convert.ToBase64String(CompressToByteArray(input));
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        public static byte[] CompressToByteArray(string input)
        {
            return CompressToByteArray(Encoding.UTF8.GetBytes(input));
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        public static byte[] CompressToByteArray(byte[] input)
        {
            byte[] buffer = input;
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return gZipBuffer;
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        public static string DecompressToString(string compressedText)
        {
            return Encoding.UTF8.GetString(DecompressToByteArray(Convert.FromBase64String(compressedText)));
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        public static string DecompressToString(byte[] compressedInput)
        {
            return Encoding.UTF8.GetString(DecompressToByteArray(compressedInput));
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public static byte[] DecompressToByteArray(string compressedText)
        {
            return DecompressToByteArray(Convert.FromBase64String(compressedText));
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public static byte[] DecompressToByteArray(byte[] input)
        {
            byte[] gZipBuffer = input;
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return buffer;
            }
        }
    }
}
