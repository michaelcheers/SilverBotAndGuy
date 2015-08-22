using System;
using System.IO;

namespace SilverBotAndGuy
{
    public static partial class ExtensionMethods
    {
        public static void WriteArray(this BinaryWriter writer, byte[] array)
        {
            writer.Write(array.Length);
            writer.Write(array);
        }
        public static void WriteArray(this BinaryWriter writer, byte[][] array)
        {
            writer.Write(array.Length);
            foreach (var item in array)
            {
                writer.WriteArray(item);
            }
        }
        public static void WriteArray (this BinaryWriter writer, byte[][][] array)
        {
            writer.Write(array.Length);
            foreach (var item in array)
            {
                writer.WriteArray(item);
            }
        }
        public static byte[] ReadByteArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] result = new byte[length];
            for (int n = 0; n < length; n++)
            {
                result[n] = reader.ReadByte();
            }
            return result;
        }
        public static byte[][] ReadDoubleByteArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[][] result = (byte[][])Array.CreateInstance(typeof(byte[]), length);
            for (int n = 0; n < length; n++)
            {
                result[n] = reader.ReadByteArray();
            }
            return result;
        }
        public static byte[][][] ReadTripleByteArray (this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[][][] result = (byte[][][])Array.CreateInstance(typeof(byte[][]), length);
            for (int n = 0; n < length; n++)
            {
                result[n] = reader.ReadDoubleByteArray();
            }
            return result;
        }
    }
}
