using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public enum Direction4D
    {
        None = 0,
        Up = 1,
        Down = -1,
        Left = 2,
        Right = -2,
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
        Block[,] blocks;
        InputState inputState = new InputState();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LaserbeamManager laserManager;
        GameTextures textures;

        public static Random rand = new Random();

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

            laserManager = new LaserbeamManager(Content);
            textures = new GameTextures(Content);

            base.LoadContent();
        }
        public Rectangle GetPosition (int x, int y)
        {
            return new Rectangle(x * widthOfBlock, y * heightOfBlock, widthOfBlock, heightOfBlock);
        }
        const int widthOfBlock = 32;
        const int heightOfBlock = 32;
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
                        spriteBatch.Draw(textures.crate, GetPosition(x, y), Color.White);
                    }
                    else if (current.HasFlag(Block.Bomb))
                    {
                        spriteBatch.Draw(textures.bomb, GetPosition(x, y), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(textures.floor, GetPosition(x, y), Color.White);
                    }
                }
            }

            for (int x = 1; x < 10; x++ )
            {
                for (int y = 8; y < 14; y++ )
                {
                    spriteBatch.Draw(textures.floor, GetPosition(x, y), Color.White);
                }
            }
            spriteBatch.Draw(textures.wall, GetPosition(8, 11), Color.White);
            spriteBatch.Draw(textures.wall, GetPosition(8, 12), Color.White);
            spriteBatch.Draw(textures.panel, GetPosition(4, 12), Color.White);
            spriteBatch.Draw(textures.crate, GetPosition(2, 12), Color.White);

            spriteBatch.Draw(textures.laserGun.Get(Direction4D.Right), GetPosition(2, 10), Color.White);
            spriteBatch.Draw(textures.dozerBot.Get(botDirection), GetPosition(0, 0), Color.White);
            Rectangle silverBotPos = GetPosition(5, 9);
            spriteBatch.Draw(textures.silverBot.Get(botDirection), new Vector2(silverBotPos.X, silverBotPos.Y), Color.White);

            laserManager.Draw(spriteBatch);
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

            laserManager.Update();
        }
    }
}
