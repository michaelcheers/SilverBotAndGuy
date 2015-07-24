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
