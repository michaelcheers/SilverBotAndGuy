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
        LaserGunRight = 0xF / (2 * 2),
        LaserGunDown = 0xF / (2),
        LaserGunLeft = 0xF,
        LaserGunUp = 0xF * 2
    }
}
