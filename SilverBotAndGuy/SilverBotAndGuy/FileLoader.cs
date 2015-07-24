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
        public static Block[,] ReadFile (Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            uint width = reader.ReadUInt32();
            uint height = reader.ReadUInt32();
            Block[,] result = new Block[width, height];
            for (uint x = 0; x < width; x++)
            {
                for (uint y = 0; y < height; y++)
                {
                    result[x, y] = (Block)stream.ReadByte();
                }
            }
            return result;
        }
    }
}
