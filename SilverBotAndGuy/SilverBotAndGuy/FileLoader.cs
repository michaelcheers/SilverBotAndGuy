using System;
using System.IO;
using System.IO.Compression;

namespace SilverBotAndGuy
{
    static class FileLoader
    {
        internal static Block[,] ReadFile (Stream stream, out byte[][][] solutions, out System.Version version, out uint startDozerBotX, out uint startDozerBotY, out bool silverBot, out uint silverBotX, out uint silverBotY)
        {
            solutions = (byte[][][])Array.CreateInstance(typeof(byte[][]), 0);
            silverBotX = default(uint);
            silverBotY = default(uint);
            BinaryReader reader = new BinaryReader(new DeflateStream(stream, CompressionMode.Decompress));
            version = reader.ReadVersion();
            uint width = reader.ReadUInt32();
            uint height = reader.ReadUInt32();
            if (version >= new System.Version(1, 0, 0, 2))
                solutions = reader.ReadTripleByteArray();
            bool isSilverBot = reader.ReadBoolean();
            silverBot = isSilverBot;
            if (isSilverBot)
            {
                silverBotX = reader.ReadUInt32();
                silverBotY = reader.ReadUInt32();
            }
            startDozerBotX = reader.ReadUInt32();
            startDozerBotY = reader.ReadUInt32();
            Block[,] result = new Block[width, height];
            for (uint x = 0; x < width; x++)
            {
                for (uint y = 0; y < height; y++)
                {
                    Block current = (Block)reader.ReadByte();
                    result[x, y] = current;
                }
            }
            reader.Close();
            return result;
        }
    }
}
