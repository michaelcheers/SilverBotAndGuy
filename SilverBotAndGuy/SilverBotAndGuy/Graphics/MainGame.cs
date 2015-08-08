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
    public enum Direction4D : sbyte
    {
        None = 0,
        Up = 1,
        Left = 2,
        Right = -2,
        Down = -1,
    }

    partial class MainGame : Game
    {
        public static SpriteFont Font;
        internal static SilverBotMirrorPosition[] GetSilverBotPositions (Vector2 position)
        {
            return new SilverBotMirrorPosition[] {
                new SilverBotMirrorPosition(position, Direction4D.Left, Direction4D.Up),
                new SilverBotMirrorPosition(position + new Vector2(1, 0), Direction4D.Right, Direction4D.Up),
                new SilverBotMirrorPosition(position + new Vector2(0, 1), Direction4D.Down, Direction4D.Left),
                new SilverBotMirrorPosition(position + new Vector2(1), Direction4D.Right, Direction4D.Down)};
        }
        internal static void FireLaser (uint x, uint y, uint width, uint height, Direction4D direction, Block[,] grid, ContentManager Content, List<Laserbeam> lasers, uint silverBotX, uint silverBotY)
        {
            SilverBotMirrorPosition[] positions = new SilverBotMirrorPosition[0];
            if (silverBotX != uint.MaxValue)
                positions = GetSilverBotPositions(new Vector2(silverBotX, silverBotY));
            Laserbeam current = new Laserbeam(new LaserbeamTextures(Content));
            current.SetStartDirection(direction);
            Vector2 currentPosition = new Vector2(x, y) + direction.ToVector2();
            while(true)
            {
                if (currentPosition.X == -1 || currentPosition.Y == -1)
                    break;
                if (currentPosition.X == width - 1)
                    break;
                if (currentPosition.Y == height - 1)
                    break;
                foreach (var item in positions)
                {
                    if (item == currentPosition)
                    {
                        direction = item.Mirror(direction.Reverse());
                        break;
                    }
                }
                if(grid[(int)currentPosition.X, (int)currentPosition.Y].LaserPassesThrough())
                {
                    current.Add(currentPosition);
                    currentPosition += direction.ToVector2();
                }
                else
                {
                    break;
                }
            }
            lasers.Add(current);
        }
        internal static void LoadLasers(uint x, uint y, uint width, uint height, Block current, ContentManager Content, Block[,] grid, List<Laserbeam> lasersList, uint silverBotX, uint silverBotY)
        {
            switch (current)
            {
                case Block.LaserGunRight:
                    {
                        FireLaser(x, y, width, height, Direction4D.Right, grid, Content, lasersList, silverBotX, silverBotY);
                        return;
                    }
                case Block.LaserGunDown:
                    {
                        FireLaser(x, y, width, height, Direction4D.Down, grid, Content, lasersList, silverBotX, silverBotY);
                        return;
                    }
                case Block.LaserGunLeft:
                    {
                        FireLaser(x, y, width, height, Direction4D.Left, grid, Content, lasersList, silverBotX, silverBotY);
                        return;
                    }
                case Block.LaserGunUp:
                    {
                        FireLaser(x, y, width, height, Direction4D.Up, grid, Content, lasersList, silverBotX, silverBotY);
                        return;
                    }
            }
        }
        public void DrawWin (PlayerAvatar player)
        {
            if (File.Exists("Levels/" + currentLevel + ".sbalvl"))
                LoadNextLevel();
            else
            {
                won = true;
            }
        }
        public void ReloadLasers (uint silverBotPositionX, uint silverBotPositionY)
        {
            laserManager.Clear();
            List<Laserbeam> lasersBeams = new List<Laserbeam>();
            for (uint x = 0; x < blocks.GetLength(0); x++)
            {
                for (uint y = 0; y < blocks.GetLength(1); y++)
                {
                    Block current = blocks[x, y];
                    LoadLasers(x, y, (uint)blocks.GetLength(0), (uint)blocks.GetLength(1), current, Content, blocks, lasersBeams, silverBotPositionX, silverBotPositionY);
                }
            }
            laserManager.AddRange(lasersBeams);
        }
        public void RestartLevel ()
        {
            currentLevel--;
            LoadNextLevel();
        }
        public void LoadLevel  ()
        {
            LoadLevel("Levels/" + currentLevel + ".sbalvl");
        }
        internal PlayerAvatar silverBot = null;
        public void LoadLevel (string location)
        {
            uint StartdozerBotX;
            uint StartdozerBotY;
            bool silverBot;
            uint StartSilverBotX;
            uint StartSilverBotY;
            blocks = FileLoader.ReadFile(File.OpenRead(location), out StartdozerBotX, out StartdozerBotY, out silverBot, out StartSilverBotX, out StartSilverBotY);
            laserManager =
                new LaserbeamManager(Content);
            dozerBot = new PlayerAvatar(textures.dozerBot, new Vector2(StartdozerBotX, StartdozerBotY), blocks, laserManager.Internal.beams, this);
            if (silverBot)
                this.silverBot = new PlayerAvatar(textures.silverBot, new Vector2(StartSilverBotX, StartSilverBotY), blocks, laserManager.Internal.beams, this, true);
            else
                this.silverBot = null;
            shadowMap = new ShadowMap(Content, blocks);
            ReloadLasers(StartSilverBotX, StartSilverBotY); 
            controlled = dozerBot;
            dozerBot.Arrive += dozerBot_Arrive;
        }

        void dozerBot_Arrive(Block block, PlayerAvatar sender, Vector2 gridPos)
        {
            if (block == Block.Panel)
            {
                controlled = silverBot;
            }
        }
        Block[,] blocks;
        InputState inputState = new InputState();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LaserbeamManager laserManager;
        internal GameTextures textures;
        PlayerAvatar dozerBot;
        ShadowMap shadowMap;

        public static Random rand = new Random();
        bool won;

        //Direction4D botDirection = Direction4D.Right;

        /// <summary>
        /// Is called when the game must start rendering the death screen.
        /// </summary>
        public void StartDrawingDeathScreen ()
        {
        }

        public MainGame () : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1;
            graphics.PreferredBackBufferHeight = 1;
        }
        protected override void Initialize()
        {
            graphics.ToggleFullScreen();
            //blocks = FileLoader.ReadFile(File.OpenRead("blocks.sbalvl"));
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
            if (Environment.GetCommandLineArgs().Length == 2)
                LoadLevel(Environment.GetCommandLineArgs()[1]);
            else
                LoadNextLevel();
            Font = Content.Load<SpriteFont>("Font");

            base.LoadContent();
        }

        public static bool IsSolid(Block b)
        {
            return b.HasFlag(Block.Wall) || b == Block.Exit;
        }

        public Rectangle GetPosition (int x, int y)
        {
            return new Rectangle(x * widthOfBlock, y * heightOfBlock, widthOfBlock, heightOfBlock);
        }
        public void LoadNextLevel()
        {
            LoadLevel();
            currentLevel++;
        }
        int currentLevel = 1;
        public const int widthOfBlock = 32;
        public const int heightOfBlock = 32;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
/*            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    Block current = blocks[x, y];
                    switch (current)
                    {
                        case Block.Ice:
                            {
                                spriteBatch.Draw(textures.ice, GetPosition(x, y));
                                break;
                            }
                        case Block.Floor:
                            {
                                spriteBatch.Draw(textures.floor, GetPosition(x, y));
                                break;
                            }
                        case Block.Exit:
                            {
                                spriteBatch.Draw(textures.exit, GetPosition(x, y));
                                break;
                            }
                        case Block.Wall:
                            {
                                spriteBatch.Draw(textures.wall, GetPosition(x, y));
                                break;
                            }
                        case Block.LaserProofWall:
                            {
                                spriteBatch.Draw(textures.laserProofWall, GetPosition(x, y));
                                break;
                            }
                        case Block.LaserGunRight:
                            {
                                spriteBatch.Draw(textures.laserGun, Direction4D.Right, GetPosition(x, y));
                                break;
                            }
                        case Block.LaserGunDown:
                            {
                                spriteBatch.Draw(textures.laserGun, Direction4D.Down, GetPosition(x, y));
                                break;
                            }
                        case Block.LaserGunLeft:
                            {
                                spriteBatch.Draw(textures.laserGun, Direction4D.Left, GetPosition(x, y));
                                break;
                            }
                        case Block.LaserGunUp:
                            {
                                spriteBatch.Draw(textures.laserGun, Direction4D.Up, GetPosition(x, y));
                                break;
                            }
                        case Block.Bomb:
                            {
                                spriteBatch.Draw(textures.bomb, GetPosition(x, y));
                                break;
                            }
                        case Block.Panel:
                            {
                                spriteBatch.Draw(textures.panel, GetPosition(x, y));
                                break;
                            }
                        case Block.Crate:
                            {
                                spriteBatch.Draw(textures.floor, GetPosition(x, y));
                                spriteBatch.Draw(textures.crate, GetPosition(x, y));
                                break;
                            }
                    }
                }
            }

            shadowMap.Draw(spriteBatch);

            for (int x = 1; x < 10; x++ )
            {
                for (int y = 8; y < 14; y++ )
                {
                    spriteBatch.Draw(textures.floor, GetPosition(x, y));
                }
            }
            spriteBatch.Draw(textures.wall, GetPosition(8, 11));
            spriteBatch.Draw(textures.wall, GetPosition(8, 12));
            spriteBatch.Draw(textures.panel, GetPosition(4, 12));
            spriteBatch.Draw(textures.crate, GetPosition(2, 12));
            shadowMap.Draw(spriteBatch);

            spriteBatch.Draw(textures.laserGun.Get(Direction4D.Right), GetPosition(2, 10));*/
            //Rectangle silverBotPos = GetPosition(5, 9);
            //spriteBatch.Draw(textures.silverBot.Get(botDirection), new Vector2(silverBotPos.X, silverBotPos.Y));

            dozerBot.Draw(spriteBatch);
            if (silverBot != null)
            {
                silverBot.Draw(spriteBatch);
            }

            laserManager.Draw(spriteBatch);
            if (won)
            {
                const string winText = "You Won!";
                spriteBatch.DrawStringCentered(GraphicsDevice, Font, winText, Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        PlayerAvatar controlled;

        void UpdateIce (Point position)
        {
            foreach (var item in laserManager.Internal.beams)
            {
                foreach (var item2 in item.Internal.squares)
                {
                    if ((item2.pos / 32) == position.ToVector2())
                        blocks[position.X, position.Y] = Block.Floor;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    if (blocks[x, y] == Block.Ice)
                    {
                        UpdateIce(new Point(x, y));
                    }
                }
            }

            inputState.Update();

            if (controlled == silverBot)
            {
                if (inputState.WasKeyJustPressed(Keys.Escape))
                    controlled = dozerBot;
                else
                    dozerBot.Update(inputState.GetPseudoJoystick(Keys.Up, Keys.Down, Keys.Left, Keys.Right), false);
            }
            else if (controlled == dozerBot)
            {
                if (silverBot != null)
                    silverBot.Update(inputState.GetPseudoJoystick(Keys.Up, Keys.Down, Keys.Left, Keys.Right), false);
            }

            controlled.Update(inputState.GetPseudoJoystick(Keys.Up, Keys.Down, Keys.Left, Keys.Right));
            laserManager.Update();
        }
    }
}
