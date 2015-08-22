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

        public static implicit operator LaserbeamTextures (ContentManager Content)
        {
            return new LaserbeamTextures(Content);
        }
    }

    public class LaserbeamManager
    {
        internal class LaserbeamManagerInternal
        {
            internal LaserbeamManagerInternal (LaserbeamManager laserBeamManager)
            {
                this.laserBeamManager = laserBeamManager;
            }
            LaserbeamManager laserBeamManager;
            public List<Laserbeam> beams
            {
                get
                {
                    return laserBeamManager.beams;
                }
            }
            public LaserbeamTextures textures
            {
                get
                {
                    return laserBeamManager.textures;
                }
            }
        }
        internal LaserbeamManagerInternal Internal
        {
            get
            {
                return new LaserbeamManagerInternal(this);
            }
        }
        LaserbeamTextures textures;
        List<Laserbeam> beams = new List<Laserbeam>();

        public void Clear ()
        {
            foreach (Laserbeam beam in beams)
            {
                beam.Clear();
            }
        }

        public LaserbeamManager(ContentManager Content)
        {
            textures = new LaserbeamTextures(Content);

            /*Laserbeam test = new Laserbeam(textures);
            test.SetStartDirection(Direction4D.Right);
            test.Add(new Vector2(3, 10));
            test.Add(new Vector2(4, 10));
            test.Add(new Vector2(5, 10));
            test.Add(new Vector2(5, 11));
            test.Add(new Vector2(5, 12));
            test.Add(new Vector2(6, 12));
            test.Add(new Vector2(7, 12));
            beams.Add(test);*/
        }

        internal Laserbeam GetBeam(int laserIndex)
        {
            while (beams.Count <= laserIndex)
            {
                beams.Add(new Laserbeam(textures));
            }

            return beams[laserIndex];
        }
       
        internal void Add (Laserbeam laserBeam)
        {
            beams.Add(laserBeam);
        }
        
        internal void AddRange (IEnumerable<Laserbeam> laserBeam)
        {
            beams.AddRange(laserBeam);
        }

        public void Update()
        {
            foreach(Laserbeam beam in beams)
            {
                beam.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            foreach(Laserbeam beam in beams)
            {
                beam.Draw(spriteBatch, cameraPos);
            }
        }
    }

    class Laserbeam
    {
        internal InternalLaserbeam Internal
        {
            get
            {
                return new InternalLaserbeam(this);
            }
        }
        internal class InternalLaserbeam
        {
            internal InternalLaserbeam (Laserbeam laserBeam)
            {
                this.laserBeam = laserBeam;
            }
            Direction4D startDirection
            {
                get
                {
                    return laserBeam.startDirection;
                }
            }
            internal List<LaserbeamSquare> squares
            {
                get
                {
                    return laserBeam.squares;
                }
            }
            internal List<float> pulses
            {
                get
                {
                    return laserBeam.pulses;
                }
            }
            internal LaserbeamTextures textures
            {
                get
                {
                    return laserBeam.textures;
                }
            }
            internal int nextPulseCountdown
            {
                get
                {
                    return laserBeam.nextPulseCountdown;
                }
            }
            Laserbeam laserBeam;
        }
        internal class LaserbeamSquare
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

            public void Draw(LaserbeamTextures textures, SpriteBatch spriteBatch, Vector2 cameraPos)
            {
                if( startDirection != endDirection )
                {
                    DrawPartial(startDirection.Reverse(), textures, spriteBatch, cameraPos);
                    DrawPartial(endDirection, textures, spriteBatch, cameraPos);
                }
                else if (startDirection == Direction4D.Up || startDirection == Direction4D.Down)
                {
                    spriteBatch.Draw(textures.beamV, pos - cameraPos, Color.White);
                }
                else // left/right
                {
                    spriteBatch.Draw(textures.beamH, pos - cameraPos, Color.White);
                }
            }

            public void DrawPartial(Direction4D direction, LaserbeamTextures textures, SpriteBatch spriteBatch, Vector2 cameraPos)
            {
                switch (direction)
                {
                    case Direction4D.Up:
                        spriteBatch.Draw(textures.beamV, pos - cameraPos, new Rectangle(0, 0, 32, 16), Color.White);
                        break;
                    case Direction4D.Down:
                        spriteBatch.Draw(textures.beamV, pos + new Vector2(0, 16.0f) - cameraPos, new Rectangle(0, 16, 32, 16), Color.White);
                        break;
                    case Direction4D.Left:
                        spriteBatch.Draw(textures.beamH, pos - cameraPos, new Rectangle(0, 0, 16, 32), Color.White);
                        break;
                    case Direction4D.Right:
                        spriteBatch.Draw(textures.beamH, pos + new Vector2(16.0f, 0.0f) - cameraPos, new Rectangle(16, 0, 16, 32), Color.White);
                        break;
                }
            }

            public void DrawPulse(LaserbeamTextures textures, float position, SpriteBatch spriteBatch, Vector2 cameraPos)
            {
                float offsetPosition = position - 0.5f;
                Direction4D relevantDirection = offsetPosition < 0 ? startDirection: endDirection;
                switch (relevantDirection)
                {
                    case Direction4D.Down:
                        spriteBatch.Draw(textures.pulseV, pos + new Vector2(0, offsetPosition * 32.0f) - cameraPos, Color.White);
                        break;
                    case Direction4D.Up:
                        spriteBatch.Draw(textures.pulseV, pos + new Vector2(0, -offsetPosition * 32.0f) - cameraPos, Color.White);
                        break;
                    case Direction4D.Left:
                        spriteBatch.Draw(textures.pulseH, pos + new Vector2(-offsetPosition * 32.0f, 0.0f) - cameraPos, Color.White);
                        break;
                    case Direction4D.Right:
                        spriteBatch.Draw(textures.pulseH, pos + new Vector2(offsetPosition * 32.0f, 0.0f) - cameraPos, Color.White);
                        break;
                }
            }
        }

        Direction4D startDirection;
        Direction4D endDirection;
        List<LaserbeamSquare> squares = new List<LaserbeamSquare>();
        List<float> pulses = new List<float>();
        LaserbeamTextures textures;
        int nextPulseCountdown;

        public Laserbeam(LaserbeamTextures aTextures)
        {
            startDirection = Direction4D.Right;
            textures = aTextures;
        }

        public void Clear()
        {
            startDirection = Direction4D.Right;
            squares.Clear();
        }

        public void SetStartDirection(Direction4D direction)
        {
            startDirection = direction;
        }

        public void SetEndDirection(Direction4D direction)
        {
            if( squares.Count() > 0)
            {
                squares.Last().endDirection = direction;
            }
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

            squares.Add(new LaserbeamSquare(newCoord, newDirection, endDirection));
        }

        public void Update()
        {
            for(int idx = 0; idx < pulses.Count; idx++)
            {
                float PULSE_SPEED = 0.1f;
                pulses[idx] += PULSE_SPEED;

                if( pulses[idx] > 50 )//squares.Count )
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

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos)
        {
            foreach(LaserbeamSquare square in squares)
            {
                square.Draw(textures, spriteBatch, cameraPos);
            }

            foreach(float f in pulses)
            {
                int index = (int)f;
                if (index >= 0 && index < squares.Count)
                    squares[index].DrawPulse(textures, f - index, spriteBatch, cameraPos);
            }
        }
    }
}
