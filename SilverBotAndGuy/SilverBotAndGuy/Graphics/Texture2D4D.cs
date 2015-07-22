using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy.Graphics
{
    class Texture2D4D
    {
        Texture2D up;
        Texture2D down;
        Texture2D left;
        Texture2D right;

        public Texture2D4D(ContentManager Content, string name)
        {
            up = Content.Load<Texture2D>(name + "_u");
            down = Content.Load<Texture2D>(name + "_d");
            left = Content.Load<Texture2D>(name + "_l");
            right = Content.Load<Texture2D>(name + "_r");
        }

        public Texture2D Get(Direction4D direction)
        {
            switch(direction)
            {
                case Direction4D.Up: return up;
                case Direction4D.Down: return down;
                case Direction4D.Left: return left;
                case Direction4D.Right: return right;
            }
            return null;
        }
    }
}
