using Microsoft.Xna.Framework;

namespace SilverBotAndGuy
{
    partial class MainGame
    {
        internal class SilverBotMirrorPosition
        {
            public Direction4D d1;
            public Direction4D d2;
            public Vector2 position;
            public SilverBotMirrorPosition (Vector2 position, Direction4D d1, Direction4D d2)
            {
                this.position = position;
                this.d1 = d1;
                this.d2 = d2;
            }
            public Direction4D Mirror (Direction4D value)
            {
                if (value == d1)
                    return d2;
                if (value == d2)
                    return d1;
                //throw new ArgumentException(value + " is not valid.");
                return Mirror(value.Reverse());
            }
            public static implicit operator Vector2 (SilverBotMirrorPosition value)
            {
                return value.position;
            }
        }
    }
}
