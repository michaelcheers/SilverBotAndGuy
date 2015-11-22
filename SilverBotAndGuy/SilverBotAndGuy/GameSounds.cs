using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    partial class MainGame
    {
        internal class GameSounds
        {
            public SilverSound cratePush;
            public SilverSound death;
            public SilverSound iceMelt;
            SilverSound backgroundMusic;

            public GameSounds (ContentManager Content)
            {
                cratePush = new SilverSound("Crate Drag 1");
                death = new SilverSound("Death");
                iceMelt = new SilverSound("Ice Melt");
                backgroundMusic = new SilverSound("Background");
                if (System.IO.File.Exists(backgroundMusic.player.SoundLocation))
                backgroundMusic.player.PlayLooping();
            }
        }
    }
}
