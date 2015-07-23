using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
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

        public PlayerAvatar(Texture2D4D aTextures, Vector2 initialGridPos)
        {
            gridPos = initialGridPos;
            animPos = gridPos * 32.0f;
            animTarget = animPos;
            textures = aTextures;
            facing = Direction4D.Right;
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

        public bool TryStep(Direction4D moveDirection)
        {
            if (moveDirection != Direction4D.None) // TODO: check for collisions
            {
                facing = moveDirection;
                gridPos += moveDirection.ToVector2();
                animTarget = gridPos * 32.0f;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures.Get(facing), animPos, Color.White);
        }
    }
}
