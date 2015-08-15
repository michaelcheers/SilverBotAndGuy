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
    public static class ExtensionMethods
    {
        public static Vector2 AddFloat(this Vector2 value, float add)
        {
            if (value.X != 0)
                value.X += add;
            if (value.Y != 0)
                value.Y += add;
            return value;
        }
        internal static PlayerAvatar.BlockMovementState GetMovementState (this Block block)
        {
            PlayerAvatar.BlockMovementState state = 0;
            if (block.IsSolid())
            {
                switch (block)
                {
                    case Block.Exit:
                        {
                            state |= PlayerAvatar.BlockMovementState.Exit;
                            break;
                        }
                    case Block.Panel:
                        {
                            state |= PlayerAvatar.BlockMovementState.Panel;
                            break;
                        }
                    case Block.Crate:
                        {
                            state |= PlayerAvatar.BlockMovementState.CratePush;
                            break;
                        }
                    default:
                        {
                            state |= PlayerAvatar.BlockMovementState.Blocked;
                            break;
                        }
                }
            }
            return state;
        }
        internal static void DrawStringCentered (this SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, SpriteFont Font, string text, Color color)
        {
            int x = (GraphicsDevice.Viewport.Width - (int)Font.MeasureString(text).X) / 2;
            int y = (GraphicsDevice.Viewport.Height - (int)Font.MeasureString(text).Y) / 2;
            spriteBatch.DrawString(Font, text, new Vector2(x, y), color);
        }
        internal static bool LaserPassesThrough (this Block block)
        {
            return !block.IsSolid() || block == Block.Wall || block == Block.Ice;
        }
        internal static T GetElement<T> (this T[,] value, Vector2 position)
        {
            return value[(int)position.X, (int)position.Y];
        }
        internal static T GetElement<T>(this T[,] value, Point position)
        {
            return value[(int)position.X, (int)position.Y];
        }
        internal static bool IsSolid (this Block block)
        {
            return MainGame.IsSolid(block);
        }
        internal static bool CastsShadows (this Block block)
        {
            return block == Block.Wall || block == Block.LaserProofWall;
        }
        public static Vector2 ToVector2(this Direction4D self)
        {
            switch (self)
            {
                case Direction4D.Up: return new Vector2(0, -1);
                case Direction4D.Down: return new Vector2(0, 1);
                case Direction4D.Left: return new Vector2(-1, 0);
                case Direction4D.Right: return new Vector2(1, 0);
                default: return new Vector2(0, 0);
            }
        }

        public static Direction4D Reverse(this Direction4D self)
        {
            switch (self)
            {
                case Direction4D.Up: return Direction4D.Down;
                case Direction4D.Down: return Direction4D.Up;
                case Direction4D.Left: return Direction4D.Right;
                case Direction4D.Right: return Direction4D.Left;
                default: return Direction4D.None;
            }
        }
        public static void Draw (this SpriteBatch spriteBatch, Texture2D texture, Rectangle rect)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }
        public static void Draw (this SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
        internal static void Draw (this SpriteBatch spriteBatch, Texture2D4D texture, Direction4D direction, Rectangle rect)
        {
            spriteBatch.Draw(texture.Get(direction), rect);
        }
        internal static void Draw(this SpriteBatch spriteBatch, Texture2D4D texture, Direction4D direction, Vector2 position)
        {
            spriteBatch.Draw(texture.Get(direction), position);
        }
    }
}
