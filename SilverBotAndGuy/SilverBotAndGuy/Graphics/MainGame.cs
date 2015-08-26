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
        internal void FireLaser (uint x, uint y, uint width, uint height, Direction4D direction, Block[,] grid, ContentManager Content, LaserbeamManager laserManager, ref int laserIndex)
        {
            SilverBotMirrorPosition[] mirrorPositions = new SilverBotMirrorPosition[0];
            if (silverBot != null)
                mirrorPositions = GetSilverBotPositions(new Vector2(silverBot.gridPos.X, silverBot.gridPos.Y));
            Laserbeam current = laserManager.GetBeam(laserIndex);
            current.SetStartDirection(direction);
            Vector2 currentPosition = new Vector2(x, y) + direction.ToVector2();
            while(true)
            {
                if (currentPosition.X == -1 || currentPosition.Y == -1)
                    break;
                if (currentPosition.X == width)
                    break;
                if (currentPosition.Y == height)
                    break;
                foreach (var mirror in mirrorPositions)
                {
                    if (mirror == currentPosition)
                    {
                        direction = mirror.Mirror(direction.Reverse());
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
            current.SetEndDirection(direction);
            laserIndex++;
        }
        internal void LoadLasers(uint x, uint y, uint width, uint height, Block current, ContentManager Content, Block[,] grid, LaserbeamManager laserManager, ref int laserIndex )
        {
            switch (current)
            {
                case Block.LaserGunRight:
                    {
                        FireLaser(x, y, width, height, Direction4D.Right, grid, Content, laserManager, ref laserIndex);
                        return;
                    }
                case Block.LaserGunDown:
                    {
                        FireLaser(x, y, width, height, Direction4D.Down, grid, Content, laserManager, ref laserIndex);
                        return;
                    }
                case Block.LaserGunLeft:
                    {
                        FireLaser(x, y, width, height, Direction4D.Left, grid, Content, laserManager, ref laserIndex);
                        return;
                    }
                case Block.LaserGunUp:
                    {
                        FireLaser(x, y, width, height, Direction4D.Up, grid, Content, laserManager, ref laserIndex);
                        return;
                    }
            }
        }
        public void DrawWin (PlayerAvatar player)
        {
            if (outS != null)
            {
                byte[][] array = outS.GetBytes();
                BinaryWriter writer = new BinaryWriter(File.OpenWrite(outSolution));
                writer.WriteArray(array);
                writer.Flush();
                writer.Close();
            }
            if (File.Exists("Levels/" + (currentLevel + 1) +".sbalvl"))
                LoadNextLevel();
            else
            {
                won = true;
            }
        }
        public void ReloadLasers ()
        {
            laserManager.Clear();
            int laserIndex = 0;
            for (uint x = 0; x < blocks.GetLength(0); x++)
            {
                for (uint y = 0; y < blocks.GetLength(1); y++)
                {
                    Block current = blocks[x, y];
                    LoadLasers(x, y, (uint)blocks.GetLength(0), (uint)blocks.GetLength(1), current, Content, blocks, laserManager, ref laserIndex);
                }
            }
        }
        public void RestartLevel ()
        {
            LoadLevel();
        }
        public void LoadLevel  ()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                LoadLevel(Environment.GetCommandLineArgs()[1]);
            }
            else
            {
                LoadLevel("Levels/" + currentLevel + ".sbalvl");
            }
        }

        internal PlayerAvatar silverBot = null;
        System.Version version;

        public void LoadLevel (string location)
        {
            pushAnims.Clear();
            uint StartdozerBotX;
            uint StartdozerBotY;
            bool silverBot;
            uint StartSilverBotX;
            uint StartSilverBotY;
            byte[][][] solutions;
            blocks = FileLoader.ReadFile(File.OpenRead(location), out solutions, out version, out StartdozerBotX, out StartdozerBotY, out silverBot, out StartSilverBotX, out StartSilverBotY);
            laserManager =
                new LaserbeamManager(Content);
            dozerBot = new PlayerAvatar(textures.dozerBot, new Vector2(StartdozerBotX, StartdozerBotY), blocks, laserManager.Internal.beams, this);
            if (silverBot)
                this.silverBot = new PlayerAvatar(textures.silverBot, new Vector2(StartSilverBotX, StartSilverBotY), blocks, laserManager.Internal.beams, this, true);
            else
                this.silverBot = null;
            this.solutions = new ReleaseSolution[solutions.Length];
            for (int n = 0; n < solutions.Length; n++)
            {
                this.solutions[n] = new ReleaseSolution(solutions[n]);
            }
            shadowMap = new ShadowMap(Content, blocks);
            ReloadLasers(); 
            controlled = dozerBot;
            dozerBot.Arrive += dozerBot_Arrive;
        }

        void dozerBot_Arrive(List<Block> blocks, PlayerAvatar sender, Vector2 gridPos)
        {
            if (blocks.Contains(Block.Panel))
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
        ReleaseSolution[] solutions;
        Vector2 cameraPos = new Vector2(-5*32, -5*32);
        List<PushAnim> pushAnims = new List<PushAnim>();

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
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
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

        CreateSolution outS = null;
        string outSolution;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            laserManager = new LaserbeamManager(Content);
            textures = new GameTextures(Content);
            if (Environment.GetCommandLineArgs().Length == 3)
            {
                LoadLevel(Environment.GetCommandLineArgs()[1]);
                outSolution = Environment.GetCommandLineArgs()[2];
                outS = new CreateSolution();
            }
            else if (Environment.GetCommandLineArgs().Length == 2)
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
            return new Rectangle((int)((x * widthOfBlock) - cameraPos.X),
                (int)((y * heightOfBlock) - cameraPos.Y),
                widthOfBlock,
                heightOfBlock
            );
        }
        public void LoadNextLevel()
        {
            currentLevel++;
            LoadLevel();
        }
        int currentLevel = 0;
        public const int widthOfBlock = 32;
        public const int heightOfBlock = 32;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (controlled != null)
            {
                cameraPos = new Vector2(
                    (float)Math.Floor(controlled.animPos.X - graphics.GraphicsDevice.Viewport.Width * 0.5f),
                    (float)Math.Floor(controlled.animPos.Y - graphics.GraphicsDevice.Viewport.Height * 0.5f)
                );
            }

            spriteBatch.Begin();
            for (int x = 0; x < blocks.GetLength(0); x++)
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
                                bool isAnimating = false;
                                foreach (PushAnim anim in pushAnims)
                                {
                                    if(x == anim.destinationGridPos.X && y == anim.destinationGridPos.Y)
                                    {
                                        isAnimating = true;
                                        break;
                                    }
                                }

                                if( !isAnimating )
                                    spriteBatch.Draw(textures.crate, GetPosition(x, y));
                                break;
                            }
                    }
                }
            }

            foreach(PushAnim anim in pushAnims)
            {
                anim.Draw(spriteBatch, cameraPos);
            }

            shadowMap.Draw(spriteBatch, cameraPos);

            /*
            for (int x = 1; x < 10; x++)
            {
                for (int y = 8; y < 14; y++)
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

            dozerBot.Draw(spriteBatch, cameraPos);
            
            laserManager.Draw(spriteBatch, cameraPos);

            if (silverBot != null)
            {
                silverBot.Draw(spriteBatch, cameraPos);
            }

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

        ReleaseSolution current;

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
            if (current != null)
                if (!current.Update(inputState))
                    inputState.Update();
                else
                    ;
            else
                inputState.Update();

            if (inputState.WasButtonJustPressed(Buttons.Back))
                Exit();
            if (outS != null)
                outS.Add(inputState);

            if (inputState.WasKeyJustPressed(Keys.F10) || inputState.WasButtonJustPressed(Buttons.X))
            {
                if (solutions.Length > 0)
                    current = solutions[0];
                else
                    System.Media.SystemSounds.Asterisk.Play();
                RestartLevel();
            }

            if (controlled == silverBot)
            {
                if (inputState.WasKeyJustPressed(Keys.Escape) || inputState.WasButtonJustPressed(Buttons.B))
                    controlled = dozerBot;
                else
                    dozerBot.Update(inputState.GetPseudoJoystick(inputState.gamePad.ThumbSticks.Left, Keys.Up, Keys.Down, Keys.Left, Keys.Right), false);
            }
            else if (controlled == dozerBot)
            {
                if (silverBot != null)
                    silverBot.Update(inputState.GetPseudoJoystick(inputState.gamePad.ThumbSticks.Left, Keys.Up, Keys.Down, Keys.Left, Keys.Right), false);
            }

            controlled.Update(inputState.GetPseudoJoystick(inputState.gamePad.ThumbSticks.Left, Keys.Up, Keys.Down, Keys.Left, Keys.Right));
            laserManager.Update();

            for (int Idx = 0; Idx < pushAnims.Count; )
            {
                if (pushAnims[Idx].Finished())
                {
                    pushAnims.RemoveAt(Idx);
                }
                else
                {
                    Idx++;
                }
            }
        }

        internal void AddPushAnim(Block block, Point destination, PlayerAvatar pusher, Direction4D moveDirection)
        {
            switch(block)
            {
                case Block.Crate:
                    pushAnims.Add( new PushAnim(textures.crate, pusher, moveDirection.ToVector2() * widthOfBlock, destination) );
                    break;
            }
        }
    }
}
