using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    public static class Extensions
    {
        public static Direction4D Reverse(this Direction4D self)
        {
            switch (self)
            {
                case Direction4D.Up: return Direction4D.Down;
                case Direction4D.Down: return Direction4D.Up;
                case Direction4D.Left: return Direction4D.Right;
                case Direction4D.Right: return Direction4D.Left;
                default: return Direction4D.None;
            }
        }
    }
}
