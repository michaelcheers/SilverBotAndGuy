using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    enum Block : byte
    {
        Floor = 0,
        Crate = 1,
        Bomb = 2,
        Panel = 3,
        Wall = 4,
        LaserProofWall = 5,
        LaserGunRight = 0xF / (2),
        LaserGunDown = 0xF,
        LaserGunLeft = 0xF * 2,
        LaserGunUp = 0xF * (2 * 2)
    }
}
