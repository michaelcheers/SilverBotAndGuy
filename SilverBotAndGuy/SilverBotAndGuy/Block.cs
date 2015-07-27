using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    enum Block : byte
    {
        Floor = 0x0,
        //0x10 or 16 is the solid bit. Anything from 0x10 to 0x1F is solid.
        Wall = 0x10,
        LaserProofWall = 0x11,
        LaserGunRight = 0x12,
        LaserGunDown = 0x13,
        LaserGunLeft = 0x14,
        LaserGunUp = 0x15,
        Bomb = 0x16,
        Panel = 0x17,
        Crate = 0x18,
    }
}
