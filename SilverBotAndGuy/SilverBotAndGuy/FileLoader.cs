using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    static class FileLoader
    {
        internal static Block[,] ReadFile (Stream stream, out System.Version version, out uint startDozerBotX, out uint startDozerBotY, out bool silverBot, out uint silverBotX, out uint silverBotY)
        {
            silverBotX = default(uint);
            silverBotY = default(uint);
            BinaryReader reader = new BinaryReader(stream);
            version = reader.ReadVersion();
            uint width = reader.ReadUInt32();
            uint height = reader.ReadUInt32();
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
                    Block current = (Block)stream.ReadByte();
                    result[x, y] = current;
                }
            }
            stream.Close();
            return result;
        }
    }
}
