using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    public static class Version
    {
        public static void Write (this BinaryWriter writer, System.Version version)
        {
            writer.Write((byte)(version.Major));
            writer.Write((byte)(version.Minor));
            writer.Write((byte)(version.Build));
            writer.Write((byte)(version.Revision));
        }
        public static System.Version ReadVersion (this BinaryReader reader)
        {
            return new System.Version(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }
        public static System.Version Current
        {
            get
            {
                return new System.Version(byte1, byte2, byte3, byte4);
            }
        }
        public const byte byte1 = 1;
        public const byte byte2 = 0;
        public const byte byte3 = 0;
        public const byte byte4 = 2;
    }
}
