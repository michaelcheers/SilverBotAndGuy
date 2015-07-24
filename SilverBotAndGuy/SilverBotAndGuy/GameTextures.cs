using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    partial class MainGame
    {
        class GameTextures
        {
            public Texture2D4D laserGun;
            public Texture2D4D dozerBot;
            public Texture2D4D silverBot;
            public Texture2D laserRight;
            public Texture2D laserDown;
            public Texture2D laserPulse;
            public Texture2D crate;
            public Texture2D bomb;
            public Texture2D floor;
            public Texture2D wall;
            public Texture2D panel;

            public GameTextures(ContentManager Content)
            {
                floor = Content.Load<Texture2D>("floor");
                wall = Content.Load<Texture2D>("wall");
                crate = Content.Load<Texture2D>("crate");
                panel = Content.Load<Texture2D>("panel");
                bomb = Content.Load<Texture2D>("bomb");
                laserRight = Content.Load<Texture2D>("laser-right");
                laserDown = Content.Load<Texture2D>("laser-down");
                laserPulse = Content.Load<Texture2D>("laser-pulse");
                laserGun = new Texture2D4D(Content, "lasergun");
                dozerBot = new Texture2D4D(Content, "dozerbot");
                silverBot = new Texture2D4D(Content, "mirrorbot");
            }
        }
    }
}
