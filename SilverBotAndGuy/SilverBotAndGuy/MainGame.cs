using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        LaserRight = 4,
        LaserDown = 8,
        LaserLeft = 16,
        LaserUp = 32
    }
    class MainGame : Game
    {
        Texture2D laserRight;
        Texture2D laserDown;
        Texture2D laserPulse;
        Texture2D crate;
        Texture2D bomb;
        Texture2D floor;
        Block[,] blocks;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public MainGame () : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            blocks = new Block[,] 
            { 
            {Block.Crate, Block.Crate, Block.Floor},
            {Block.Bomb, Block.Floor, Block.Crate}
            };
            base.Initialize();
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            Process.GetCurrentProcess().Kill();
            base.OnExiting(sender, args);
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            floor = Content.Load<Texture2D>("floor");
            crate = Content.Load<Texture2D>("crate");
            bomb = Content.Load<Texture2D>("bomb");
            laserRight = Content.Load<Texture2D>("laser-right");
            laserDown = Content.Load<Texture2D>("laser-down");
            laserPulse = Content.Load<Texture2D>("laser-pulse");

            base.LoadContent();
        }
        public Rectangle GetPosition (int x, int y)
        {
            return new Rectangle(x * widthOfBlock, y * heightOfBlock, widthOfBlock, heightOfBlock);
        }
        const int widthOfBlock = 64;
        const int heightOfBlock = 64;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    Block current = blocks[x, y];
                    if (current.HasFlag(Block.Crate))
                    {
                        spriteBatch.Draw(crate, GetPosition(x, y), Color.White);
                    }
                    else if (current.HasFlag(Block.Bomb))
                    {
                        spriteBatch.Draw(bomb, GetPosition(x, y), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(floor, GetPosition(x, y), Color.White);
                    }
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
