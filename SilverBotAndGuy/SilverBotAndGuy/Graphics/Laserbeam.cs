using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotAndGuy.Graphics
{
    class LaserbeamTextures
    {
        public Texture2D beamH;
        public Texture2D beamV;
        public Texture2D pulseH;
        public Texture2D pulseV;

        public LaserbeamTextures(ContentManager Content)
        {
            beamH = Content.Load<Texture2D>("laser_h");
            beamV = Content.Load<Texture2D>("laser_v");
            pulseH = Content.Load<Texture2D>("laser_pulse_h");
            pulseV = Content.Load<Texture2D>("laser_pulse_v");
        }
    }

    public class LaserbeamManager
    {
        LaserbeamTextures textures;
        List<Laserbeam> beams = new List<Laserbeam>();

        public LaserbeamManager(ContentManager Content)
        {
            textures = new LaserbeamTextures(Content);

            Laserbeam test = new Laserbeam(textures);
            test.SetStartDirection(Direction4D.Right);
            test.Add(new Vector2(3, 10));
            test.Add(new Vector2(4, 10));
            test.Add(new Vector2(5, 10));
            test.Add(new Vector2(5, 11));
            test.Add(new Vector2(5, 12));
            test.Add(new Vector2(6, 12));
            test.Add(new Vector2(7, 12));
            beams.Add(test);
        }

        public void Update()
        {
            foreach(Laserbeam beam in beams)
            {
                beam.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Laserbeam beam in beams)
            {
                beam.Draw(spriteBatch);
            }
        }
    }

    class Laserbeam
    {
        class LaserbeamSquare
        {
            public Direction4D startDirection;
            public Direction4D endDirection;
            public Vector2 coord;
            public Vector2 pos { get { return coord * 32.0f;  } }

            public LaserbeamSquare(Vector2 aCoord, Direction4D direction)
            {
                coord = aCoord;
                startDirection = direction;
                endDirection = direction;
            }

            public LaserbeamSquare(Vector2 aCoord, Direction4D aStartDirection, Direction4D aEndDirection)
            {
                coord = aCoord;
                startDirection = aStartDirection;
                endDirection = aEndDirection;
            }

            public void Draw(LaserbeamTextures textures, SpriteBatch spriteBatch)
            {
                if( startDirection != endDirection )
                {
                    DrawPartial(startDirection.Reverse(), textures, spriteBatch);
                    DrawPartial(endDirection, textures, spriteBatch);
                }
                else if (startDirection == Direction4D.Up || startDirection == Direction4D.Down)
                {
                    spriteBatch.Draw(textures.beamV, pos, Color.White);
                }
                else // left/right
                {
                    spriteBatch.Draw(textures.beamH, pos, Color.White);
                }
            }

            public void DrawPartial(Direction4D direction, LaserbeamTextures textures, SpriteBatch spriteBatch)
            {
                switch (direction)
                {
                    case Direction4D.Up:
                        spriteBatch.Draw(textures.beamV, pos, new Rectangle(0, 0, 32, 16), Color.White);
                        break;
                    case Direction4D.Down:
                        spriteBatch.Draw(textures.beamV, pos + new Vector2(0, 16.0f), new Rectangle(0, 16, 32, 16), Color.White);
                        break;
                    case Direction4D.Left:
                        spriteBatch.Draw(textures.beamH, pos, new Rectangle(0, 0, 16, 32), Color.White);
                        break;
                    case Direction4D.Right:
                        spriteBatch.Draw(textures.beamH, pos + new Vector2(16.0f, 0.0f), new Rectangle(16, 0, 16, 32), Color.White);
                        break;
                }
            }

            public void DrawPulse(LaserbeamTextures textures, float position, SpriteBatch spriteBatch)
            {
                float offsetPosition = position - 0.5f;
                Direction4D relevantDirection = offsetPosition < 0 ? startDirection: endDirection;
                switch (relevantDirection)
                {
                    case Direction4D.Down:
                        spriteBatch.Draw(textures.pulseV, pos + new Vector2(0, offsetPosition*32.0f), Color.White);
                        break;
                    case Direction4D.Up:
                        spriteBatch.Draw(textures.pulseV, pos + new Vector2(0, 32.0f-offsetPosition * 32.0f), Color.White);
                        break;
                    case Direction4D.Left:
                        spriteBatch.Draw(textures.pulseH, pos + new Vector2(32.0f - offsetPosition * 32.0f, 0.0f), Color.White);
                        break;
                    case Direction4D.Right:
                        spriteBatch.Draw(textures.pulseH, pos + new Vector2(offsetPosition * 32.0f, 0.0f), Color.White);
                        break;
                }
            }
        }

        Direction4D startDirection;
        List<LaserbeamSquare> squares = new List<LaserbeamSquare>();
        List<float> pulses = new List<float>();
        LaserbeamTextures textures;
        int nextPulseCountdown;

        public Laserbeam(LaserbeamTextures aTextures)
        {
            startDirection = Direction4D.Right;
            textures = aTextures;
            pulses.Add(0);
            pulses.Add(1);
        }

        public void Clear()
        {
            startDirection = Direction4D.Right;
            squares.Clear();
            pulses.Clear();
        }

        public void SetStartDirection(Direction4D direction)
        {
            startDirection = direction;
        }

        public void Add(Vector2 newCoord)
        {
            Direction4D newDirection = startDirection;
            if( squares.Count > 0 )
            {
                Vector2 offset = newCoord - squares.Last().coord;
                if (offset.Y == -1)
                {
                    newDirection = Direction4D.Up;
                }
                else if (offset.Y == 1)
                {
                    newDirection = Direction4D.Down;
                }
                else if (offset.X == -1)
                {
                    newDirection = Direction4D.Left;
                }
                else if (offset.X == 1)
                {
                    newDirection = Direction4D.Right;
                }

                squares.Last().endDirection = newDirection;
            }

            squares.Add(new LaserbeamSquare(newCoord, newDirection));
        }

        public void Update()
        {
            for(int idx = 0; idx < pulses.Count; idx++)
            {
                float PULSE_SPEED = 0.1f;
                pulses[idx] = pulses[idx] + PULSE_SPEED;

                if( pulses[idx] > squares.Count )
                {
                    pulses.RemoveAt(idx);
                    idx--;
                }
            }

            nextPulseCountdown--;
            if( nextPulseCountdown <= 0 )
            {
                pulses.Add(0);
                nextPulseCountdown = 15 + (int)(MainGame.rand.NextDouble() * 45);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(LaserbeamSquare square in squares)
            {
                square.Draw(textures, spriteBatch);
            }

            foreach(float f in pulses)
            {
                int index = (int)f;
                if (index >= 0 && index < squares.Count)
                    squares[index].DrawPulse(textures, f - index, spriteBatch);
            }
        }
    }
}
