using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    class PlayerAvatar
    {
        Vector2 gridPos;
        Vector2 animPos;
        Vector2 animTarget;
        Vector2 animVel;
        Texture2D4D textures;
        Direction4D facing;
        IEnumerable<Laserbeam> laserBeams;
        Block[,] grid;
        MainGame game;
        Texture2D explosionTexture;
        float explosionAnimStopwatch;
        bool isSilverBot;

        public PlayerAvatar(Texture2D4D aTextures, Vector2 initialGridPos, Block[,] grid, IEnumerable<Laserbeam> laserBeams, MainGame game, bool isSilverBot = false)
        {
            gridPos = initialGridPos;
            animPos = gridPos * 32.0f;
            animTarget = animPos;
            textures = aTextures;
            facing = Direction4D.Right;
            this.grid = grid;
            this.laserBeams = laserBeams;
            this.game = game;
            this.explosionTexture = game.textures.explosion;
            Arrive += PlayerAvatar_Arrive;
            this.isSilverBot = isSilverBot;
        }

        void PlayerAvatar_Arrive(Block block, PlayerAvatar sender, Vector2 gridPos)
        {
            if (block == Block.Exit)
            {
                game.DrawWin(this);
            }
        }

        public delegate void LandOnSquare(Block block, PlayerAvatar sender, Vector2 gridPos);

        public void Update(Vector2 joystick)
        {
            Direction4D moveDirection = Direction4D.None;
            if (explosionAnimStopwatch > 0.0f)
            {
                explosionAnimStopwatch += 3.1f;
                if( explosionAnimStopwatch > 64.0f )
                {
                    explosionAnimStopwatch = 64.0f;
                    game.RestartLevel();
                }
            }
            else
            {
                if (Math.Abs(joystick.X) > Math.Abs(joystick.Y))
                {
                    if (joystick.X > 0.5f)
                    {
                        moveDirection = Direction4D.Right;
                    }
                    else if (joystick.X < -0.5f)
                    {
                        moveDirection = Direction4D.Left;
                    }
                }
                else
                {
                    if (joystick.Y > 0.5f)
                    {
                        moveDirection = Direction4D.Down;
                    }
                    else if (joystick.Y < -0.5f)
                    {
                        moveDirection = Direction4D.Up;
                    }
                }
            }

            if( animPos == animTarget )
            {
                if (explosionAnimStopwatch == 0.0f)
                {
                    TryStep(moveDirection);
                }
            }
            else
            {
                float distSqrToTarget = (animPos - animTarget).LengthSquared();
                if (distSqrToTarget < 16.0f * 16.0f)
                {
                    if (animVel.LengthSquared() > 0.1f && moveDirection != facing )
                    {
                        animVel *= 0.95f;
                    }
                }
                else
                {
                    animVel += (animTarget - animPos) * 0.01f;
                    animVel *= 0.98f;
                }
                animPos += animVel;
                float newDistSqrToTarget = (animPos - animTarget).LengthSquared();

                if( newDistSqrToTarget > distSqrToTarget )
                {
                    bool stepped = false;

                    if( moveDirection == facing )
                    {
                        stepped = TryStep(moveDirection);
                    }

                    if (!stepped)
                    {
                        animVel = Vector2.Zero;
                        animPos = animTarget;
                        if (Arrive != null)
                            Arrive(grid[(int)gridPos.X, (int)gridPos.Y], this, gridPos);
                    }
                }
            }
        }

        public void Die ()
        {
            Explode();
        }

        PlayerAvatar currentlyControlling;

        public void Explode()
        {
            explosionAnimStopwatch = 0.01f;
            game.StartDrawingDeathScreen();
        }

        public event LandOnSquare Arrive;

        public bool TryStep(Direction4D moveDirection)
        {
            if (moveDirection != Direction4D.None)
            {
                facing = moveDirection;
                Vector2 moveTo = moveDirection.ToVector2() + gridPos;
                if (moveTo.X == -1 || moveTo.Y == -1 || moveTo.X == grid.GetLength(0) || moveTo.Y == grid.GetLength(1) || isSilverBot ? (moveTo.X == grid.GetLength(0) - 1 || moveTo.Y == grid.GetLength(1) - 1) : false)
                    return false;
                Block moveToBlock = grid[(int)moveTo.X, (int)moveTo.Y];
                Block moveToBlock1XPlus = default(Block);
                Block moveToBlock1YPlus = default(Block);
                if (isSilverBot)
                {
                    moveToBlock1XPlus = grid[(int)moveTo.X + 1, (int)moveTo.Y];
                    moveToBlock1YPlus = grid[(int)moveTo.X, (int)moveTo.Y + 1];
                }
                if (moveToBlock == Block.Crate)
                {
                    Vector2 crateMovePlus = moveDirection.ToVector2();
                    if (!grid[(int)(moveTo.X + crateMovePlus.X), (int)(moveTo.Y + crateMovePlus.Y)].IsSolid())
                    {
                        grid[(int)(moveTo.X + crateMovePlus.X), (int)(moveTo.Y + crateMovePlus.Y)] = Block.Crate;
                        grid[(int)(moveTo.X), (int)moveTo.Y] = Block.Floor;
                    }
                    else
                        return false;
                }
                if (isSilverBot)
                    goto SkipForeach;
                foreach (var item in laserBeams)
                {
                    Laserbeam.InternalLaserbeam Internal = item.Internal;
                    foreach (Laserbeam.LaserbeamSquare item2 in Internal.squares)
                    {
                        if ((item2.pos / 32) == moveTo)
                            Die();
                    }
                }
            SkipForeach:
                if ((!MainGame.IsSolid(moveToBlock) || moveToBlock == Block.Panel) && (isSilverBot ? !MainGame.IsSolid(moveToBlock1XPlus) && !MainGame.IsSolid(moveToBlock1YPlus) : true))
                {
                    gridPos = moveTo;
                    animTarget = gridPos * 32.0f;
                    game.ReloadLasers((uint)game.silverBot.gridPos.X, (uint)game.silverBot.gridPos.Y);
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures, facing, animPos);
            if( explosionAnimStopwatch > 0.0f )
            {
                spriteBatch.Draw(explosionTexture, new Rectangle(
                    (int)(animPos.X + (MainGame.widthOfBlock - explosionAnimStopwatch)/ 2.0f),
                    (int)(animPos.Y + (MainGame.heightOfBlock - explosionAnimStopwatch) / 2.0f),
                    (int)explosionAnimStopwatch,
                    (int)explosionAnimStopwatch), Color.White);
            }
        }
    }
}
