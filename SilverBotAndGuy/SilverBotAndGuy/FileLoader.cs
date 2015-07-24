using Microsoft.Xna.Framework.Content;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy
{
    static class FileLoader
    {
        internal static Block[,] ReadFile (Stream stream, ContentManager Content, out IEnumerable<Laserbeam> lasers)
        {
            List<Laserbeam> lasersList = new List<Laserbeam>();
            BinaryReader reader = new BinaryReader(stream);
            uint width = reader.ReadUInt32();
            uint height = reader.ReadUInt32();
            Block[,] result = new Block[width, height];
            for (uint x = 0; x < width; x++)
            {
                for (uint y = 0; y < height; y++)
                {
                    Block current = (Block)stream.ReadByte();
                    result[x, y] = current;
                    switch (current)
                    {
                        case Block.LaserGunRight:
                            {
                                Laserbeam beam = new Laserbeam(Content);
                                beam.SetStartDirection(Direction4D.Down);
                                if (x == width - 1)
                                    break;
                                for (int n = (int)(x); n < width; n++)
                                {
                                    beam.Add(new Microsoft.Xna.Framework.Vector2(n, y));
                                }
                                lasersList.Add(beam);
                                break;
                            }
                        case Block.LaserGunDown:
                            {
                                Laserbeam beam = new Laserbeam(Content);
                                beam.SetStartDirection(Direction4D.Down);
                                if (y == height - 1)
                                    break;
                                for (int n = (int)(y); n < height; n++)
                                {
                                    beam.Add(new Microsoft.Xna.Framework.Vector2(x, n));
                                }
                                lasersList.Add(beam);
                                break;
                            }
                        case Block.LaserGunLeft:
                            {
                                Laserbeam beam = new Laserbeam(Content);
                                beam.SetStartDirection(Direction4D.Left);
                                if (x == 0)
                                    break;
                                for (int n = (int)(x - 1); n >= 0; n--)
                                {
                                    beam.Add(new Microsoft.Xna.Framework.Vector2(n, y));
                                }
                                lasersList.Add(beam);
                                break;
                            }
                        case Block.LaserGunUp:
                            {
                                Laserbeam beam = new Laserbeam(Content);
                                beam.SetStartDirection(Direction4D.Up);
                                if (y == 0)
                                    break;
                                for (int n = (int)(y - 1); n >= 0; n-- )
                                {
                                    beam.Add(new Microsoft.Xna.Framework.Vector2(x, n));
                                }
                                lasersList.Add(beam);
                                    break;
                            }
                        default:
                            break;
                    }
                }
            }
            lasers = lasersList;
            return result;
        }
    }
}
