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
        Crate = 0x1,
        Bomb = 0x2,
        Panel = 0x3,
        Wall = 0x4,
        LaserProofWall = 0x5,
        LaserGunRight = 0x6,
        LaserGunDown = 0x7,
        LaserGunLeft = 0x8,
        LaserGunUp = 0x9
    }
}
