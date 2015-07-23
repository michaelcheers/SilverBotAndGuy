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
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            Block[,] result = new Block[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = (Block)stream.ReadByte();
                }
            }
            return result;
        }
    }
}
