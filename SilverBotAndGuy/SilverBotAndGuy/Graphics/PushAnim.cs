using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy.Graphics
{
    class PushAnim
    {
        Texture2D image;
        PlayerAvatar pusher;
        Vector2 pushOffset;
        public Point destinationGridPos;
        float closestDistSqr = 1E6f;

        public PushAnim(Texture2D image, PlayerAvatar pusher, Vector2 offset, Point destination)
        {
            this.image = image;
            this.pusher = pusher;
            this.pushOffset = offset;
            this.destinationGridPos = destination;
        }

        public bool Finished()
        {
            Vector2 offsetToTarget = pusher.animPos + pushOffset - new Vector2(destinationGridPos.X * MainGame.widthOfBlock, destinationGridPos.Y * MainGame.heightOfBlock);
            float distSqr = offsetToTarget.LengthSquared();
            if (distSqr > closestDistSqr)
                return true;

            closestDistSqr = distSqr;

            return distSqr < 1.0f;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            spriteBatch.Draw(image, pusher.animPos + pushOffset - cameraPos, Color.Blue);
        }
    }
}
