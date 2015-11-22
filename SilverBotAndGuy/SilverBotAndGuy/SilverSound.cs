using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    class SilverSound
    {
        internal SoundPlayer player;
        public SilverSound (SoundPlayer player)
        {
            this.player = player;
        }
        public SilverSound (string name) :this(new SoundPlayer("Content/Sounds/" + name + ".wav"))
        {
            
        }
        public void Play ()
        {
            player.Play();
        }
    }
}
