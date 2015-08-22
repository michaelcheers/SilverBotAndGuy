using Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    class CreateSolution
    {
        List<KeyboardState> states = new List<KeyboardState>();
        public CreateSolution ()
        {

        }
        public void Add (KeyboardState value)
        {
            states.Add(value);
        }
        public void Add (InputState value)
        {
            states.Add(value.keyboard);
        }
        public byte[][] GetBytes ()
        {
            byte[][] result = (byte[][])Array.CreateInstance(typeof(byte[]), states.Count);
            for (int n = 0; n < states.Count; n++)
            {
                result[n] = states[n].GetPressedKeys().Cast<object>().Select(o => (byte)((Keys)o)).ToArray(); ;
            }
            return result;
        }
    }
}
