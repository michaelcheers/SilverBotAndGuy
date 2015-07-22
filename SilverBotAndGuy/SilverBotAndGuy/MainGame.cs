using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    enum Direction4D
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
    }

    enum Block : byte
    {
        Floor = 0,
        Crate = 1,
        Bomb = 2,
        LaserGunRight = 4,
        LaserGunDown = 8,
        LaserGunLeft = 16,
        LaserGunUp = 32
    }
    class MainGame : Game
    {
        Texture2D4D laserGun;
        Texture2D4D dozerBot;
        Texture2D4D silverBot;
        Texture2D laserRight;
        Texture2D laserDown;
        Texture2D laserPulse;
        Texture2D crate;
        Texture2D bomb;
        Texture2D floor;
        Block[,] blocks;
        InputState inputState = new InputState();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Direction4D botDirection = Direction4D.Right;

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
         //   laserGun = new Texture2D4D(Content, "lasergun");
            dozerBot = new Texture2D4D(Content, "dozerbot");
            silverBot = new Texture2D4D(Content, "mirrorbot");

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

            spriteBatch.Draw(dozerBot.Get(botDirection), GetPosition(0, 0), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            inputState.Update();

            if( inputState.WasKeyJustPressed(Keys.Up) )
            {
                botDirection = Direction4D.Up;
            }
            else if (inputState.WasKeyJustPressed(Keys.Down))
            {
                botDirection = Direction4D.Down;
            }
            else if (inputState.WasKeyJustPressed(Keys.Left))
            {
                botDirection = Direction4D.Left;
            }
            else if (inputState.WasKeyJustPressed(Keys.Right))
            {
                botDirection = Direction4D.Right;
            }

        }
    }
}
