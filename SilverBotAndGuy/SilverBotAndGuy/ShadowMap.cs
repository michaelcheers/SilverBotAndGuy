using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    class ShadowMap
    {
        Texture2D shadow_u;
        Texture2D shadow_l;
        Texture2D shadow_ul;
        Texture2D shadow_cu;
        Texture2D shadow_cl;
        Texture2D shadow_c;
        bool[,] testGrid;

        public ShadowMap(ContentManager Content)
        {
            shadow_u = Content.Load<Texture2D>("shadow_u");
            shadow_l = Content.Load<Texture2D>("shadow_l");
            shadow_ul = Content.Load<Texture2D>("shadow_ul");
            shadow_c = Content.Load<Texture2D>("shadow_c");
            shadow_cu = Content.Load<Texture2D>("shadow_cu");
            shadow_cl = Content.Load<Texture2D>("shadow_cl");

            bool W = true;
            bool _ = false;
            testGrid = new bool[,] {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,W,_},
                {_,_,_,_,_,_,_,_,W,_},
                {_,_,_,_,_,_,_,_,_,_},
            };
        }

        public bool CastsShadow(Block b)
        {
            return (b & Block.Wall) != 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int x = 0; x < testGrid.GetLength(1); x++ )
            {
                for (int y = 0; y < testGrid.GetLength(0); y++)
                {
                    if( testGrid[y,x] )
                        continue; // never draw any shadows on a wall

                    int textureId = 0;
                    textureId += (x > 0 && testGrid[y,x - 1]) ? 1 : 0; // left side
                    textureId += (y > 0 && testGrid[y - 1,x]) ? 2 : 0; // top side
                    textureId += (x>0 && y > 0 && testGrid[y-1, x - 1]) ? 4: 0; // top left corner

                    Texture2D shadowTexture = null;
                    switch(textureId)
                    {
                        case 0: break;
                        case 1: shadowTexture = shadow_cl; break;
                        case 2: shadowTexture = shadow_cu; break;
                        case 3: shadowTexture = shadow_ul; break;
                        case 4: shadowTexture = shadow_c; break;
                        case 5: shadowTexture = shadow_l; break;
                        case 6: shadowTexture = shadow_u; break;
                        case 7: shadowTexture = shadow_ul; break;
                    }

                    if( shadowTexture != null )
                    {
                        spriteBatch.Draw(shadowTexture, new Rectangle(x * 32, y * 32 + 288, 32, 32), Color.White);
                    }
                }
            }
        }
    }
}
