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

        public PlayerAvatar(Texture2D4D aTextures, Vector2 initialGridPos, Block[,] grid, IEnumerable<Laserbeam> laserBeams)
        {
            gridPos = initialGridPos;
            animPos = gridPos * 32.0f;
            animTarget = animPos;
            textures = aTextures;
            facing = Direction4D.Right;
            this.grid = grid;
            this.laserBeams = laserBeams;
        }

        public void Update(Vector2 joystick)
        {
            Direction4D moveDirection = Direction4D.None;
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

            if( animPos == animTarget )
            {
                TryStep(moveDirection);
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
                    }
                }
            }
        }

        public void Die ()
        {
            Process.GetCurrentProcess().Kill();
        }

        public bool TryStep(Direction4D moveDirection)
        {
            if (moveDirection != Direction4D.None)
            {
                facing = moveDirection;
                Vector2 moveTo = moveDirection.ToVector2() + gridPos;
                if (moveTo.X == -1 || moveTo.Y == -1)
                    return false;
                Block moveToBlock = grid[(int)moveTo.X, (int)moveTo.Y];
                foreach (var item in laserBeams)
                {
                    Laserbeam.InternalLaserbeam Internal = item.Internal;
                    foreach (Laserbeam.LaserbeamSquare item2 in Internal.squares)
                    {
                        if ((item2.pos / 32) == moveTo)
                            Die();
                    }
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
                if (!MainGame.IsSolid(moveToBlock))
                {
                    gridPos = moveTo;
                    animTarget = gridPos * 32.0f;
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures.Get(facing), animPos);
        }
    }
}
