using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

    partial class MainGame : Game
    {
        Block[,] blocks;
        InputState inputState = new InputState();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LaserbeamManager laserManager;
        GameTextures textures;
        PlayerAvatar dozerBot;

        public static Random rand = new Random();

        Direction4D botDirection = Direction4D.Right;

        public MainGame () : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            blocks = FileLoader.ReadFile(File.OpenRead("blocks.sbalvl"));
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

            dozerBot = new PlayerAvatar(textures.dozerBot, new Vector2(2,7));

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
                    if (current.HasFlag(Block.Wall))
                    {
                        spriteBatch.Draw(textures.wall, GetPosition(x, y), Color.White);
                    }
                    else if (current.HasFlag(Block.LaserProofWall))
                    {
                        spriteBatch.Draw(textures.laserProofWall, GetPosition(x, y), Color.White);
                    }
                    else if (current.HasFlag(Block.Panel))
                    {
                        spriteBatch.Draw(textures.panel, GetPosition(x, y), Color.White);
                    }
                    else if (current.HasFlag(Block.Crate))
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
            Rectangle silverBotPos = GetPosition(5, 9);
            spriteBatch.Draw(textures.silverBot.Get(botDirection), new Vector2(silverBotPos.X, silverBotPos.Y), Color.White);

            dozerBot.Draw(spriteBatch);

            laserManager.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            inputState.Update();

            dozerBot.Update(inputState.GetPseudoJoystick(Keys.Up, Keys.Down, Keys.Left, Keys.Right));

            laserManager.Update();
        }
    }
}
