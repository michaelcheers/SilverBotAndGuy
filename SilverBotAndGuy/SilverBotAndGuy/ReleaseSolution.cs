using Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    class ReleaseSolution
    {
        KeyboardState[] states;
        int frame = 0;
        public ReleaseSolution (KeyboardState[] states)
        {
            this.states = states;
        }
        public ReleaseSolution (byte[][] input)
        {
            states = new KeyboardState[input.Length];
            for (int n = 0; n < input.Length; n++)
            {
                var item = input[n];
                states[n] = new KeyboardState(item.Cast<object>().Select(item2 => (Keys)((byte)item2)).ToArray());
            }
        }
        public bool Update (InputState state)
        {
            if (frame == states.Length)
                return false;
            state.keyboard = states[frame];
            frame++;
            return true;
        }
    }
}
