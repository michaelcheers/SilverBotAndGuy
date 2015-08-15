using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilverBotAndGuy.Graphics;
using System;
using System.Collections.Generic;

namespace SilverBotAndGuy
{
    class PlayerAvatar
    {
        Vector2 gridPos;
        Vector2 animPos;
        Vector2 animTarget;
        Vector2 animVel;
        Texture2D4D textures;
        Direction4D facing;
        IEnumerable<Laserbeam> laserBeams;
        Block[,] grid;
        MainGame game;
        Texture2D explosionTexture;
        float explosionAnimStopwatch;
        bool isSilverBot;

        public bool IsTouchingSilverBot (Vector2 posToCheck)
        {
            if (game.silverBot == null)
                return false;
            foreach (var item in MainGame.GetSilverBotPositions(game.silverBot.gridPos))
            {
                if (item == posToCheck)
                    return true;
            }
            return false;
        }

        public bool IsTouchingSilverBot ()
        {
            return IsTouchingSilverBot(gridPos);
        }

        public PlayerAvatar(Texture2D4D aTextures, Vector2 initialGridPos, Block[,] grid, IEnumerable<Laserbeam> laserBeams, MainGame game, bool isSilverBot = false)
        {
            gridPos = initialGridPos;
            animPos = gridPos * 32.0f;
            animTarget = animPos;
            textures = aTextures;
            facing = Direction4D.Right;
            this.grid = grid;
            this.laserBeams = laserBeams;
            this.game = game;
            explosionTexture = game.textures.explosion;
            Arrive += PlayerAvatar_Arrive;
            this.isSilverBot = isSilverBot;
        }

        void PlayerAvatar_Arrive(List<Block> blocksOn, PlayerAvatar sender, Vector2 gridPos)
        {
            if (blocksOn.Contains(Block.Exit))
            {
                game.DrawWin(this);
            }
        }

        public delegate void LandOnSquare(List<Block> blocksOn, PlayerAvatar sender, Vector2 gridPos);

        void DetectLaserProblem (Vector2 moveTo)
        {
            foreach (var item in laserBeams)
            {
                Laserbeam.InternalLaserbeam Internal = item.Internal;
                foreach (Laserbeam.LaserbeamSquare item2 in Internal.squares)
                {
                    if ((item2.pos / 32) == moveTo)
                        Die();
                }
            }
        }

        public void Update(Vector2 joystick, bool updateTryStep = true)
        {
            Direction4D moveDirection = Direction4D.None;
            if (explosionAnimStopwatch > 0.0f)
            {
                explosionAnimStopwatch += 3.1f;
                if( explosionAnimStopwatch > 64.0f )
                {
                    explosionAnimStopwatch = 64.0f;
                    game.RestartLevel();
                }
            }
            else
            {
                if (Math.Abs(joystick.X) > Math.Abs(joystick.Y))
                {
                    if (joystick.X > 0.5f)
                    {
                        moveDirection = Direction4D.Right;
                    }
                    else if (joystick.X < -0.5f)
                    {
                        moveDirection = Direction4D.Left;
                    }
                }
                else
                {
                    if (joystick.Y > 0.5f)
                    {
                        moveDirection = Direction4D.Down;
                    }
                    else if (joystick.Y < -0.5f)
                    {
                        moveDirection = Direction4D.Up;
                    }
                }
            }

            if( animPos == animTarget )
            {
                if (explosionAnimStopwatch == 0.0f)
                {
                    Vector2 moveTo = new Vector2();
                    if (updateTryStep)
                    {
                        TryStep(moveDirection, out moveTo);
                    }
                    DetectLaserProblem(moveTo);
                }
            }
            else
            {
                float distSqrToTarget = (animPos - animTarget).LengthSquared();
                if (distSqrToTarget < 16.0f * 16.0f)
                {
                    if (animVel.LengthSquared() > 0.1f && moveDirection != facing )
                    {
                        animVel *= 0.95f;
                    }
                }
                else
                {
                    animVel += (animTarget - animPos) * 0.01f;
                    animVel *= 0.98f;
                }
                animPos += animVel;
                float newDistSqrToTarget = (animPos - animTarget).LengthSquared();

                if( newDistSqrToTarget > distSqrToTarget )
                {
                    bool stepped = false;

                    if( moveDirection == facing )
                    {
                        Vector2 moveTo = new Vector2();
                        if (updateTryStep)
                        {
                            stepped = TryStep(moveDirection, out moveTo);
                        }
                        DetectLaserProblem(moveTo);
                    }

                    if (!stepped)
                    {
                        animVel = Vector2.Zero;
                        animPos = animTarget;
                        if (Arrive != null)
                        {
                            List<MainGame.SilverBotMirrorPosition> on;
                            if (isSilverBot)
                            {
                                on = new List<MainGame.SilverBotMirrorPosition>(MainGame.GetSilverBotPositions(gridPos));
                            }
                            else
                            {
                                on = new List<MainGame.SilverBotMirrorPosition>() {new MainGame.SilverBotMirrorPosition(gridPos, Direction4D.None, Direction4D.None) };
                            }
                            List<Block> blocks = new List<Block>();
                            foreach (var item in on)
                            {
                                blocks.Add(grid.GetElement(item));
                            }
                            Arrive(blocks, this, gridPos);
                        }
                    }
                }
            }
        }

        public void Die ()
        {
            Explode();
        }

        public void Explode()
        {
            explosionAnimStopwatch = 0.01f;
            game.StartDrawingDeathScreen();
        }

        public event LandOnSquare Arrive;

        public bool Push (Vector2 position, PlayerAvatar sender)
        {
            Point gridPosPlusPosition = (position + gridPos).ToPoint();
            if (gridPosPlusPosition.X == grid.GetLength(0) - 1 || gridPosPlusPosition.Y == grid.GetLength(1) - 1 || gridPosPlusPosition.X == -1 ||gridPosPlusPosition.Y == -1)
                return false;
            if (grid[gridPosPlusPosition.X, gridPosPlusPosition.Y].IsSolid())
                return false;
            if (isSilverBot)
            {
                if (position.X > 0 || position.Y > 0)
                {
                    Point gridPosPlusPositionPlusSilverBot = gridPosPlusPosition + new Point(1, 1);
                    if (gridPosPlusPositionPlusSilverBot.X == grid.GetLength(0) - 1 || gridPosPlusPositionPlusSilverBot.Y == grid.GetLength(1) - 1)
                        return false;
                }
                else if (position.X < 0 || position.Y < 0)
                {
                    Point gridPosPlusPositionPlusSilverBot = gridPosPlusPosition + new Point(-1, -1);
                    if (gridPosPlusPositionPlusSilverBot.X == -1 || gridPosPlusPositionPlusSilverBot.Y == -1)
                        return false;
                }
            }
            gridPos += position;
            animTarget = gridPos * 32.0f;
            return true;
        }

        public enum BlockMovementState : byte
        {
            Blocked = 1,
            Exit = 2,
            Panel = 4,
            NonBlocked = 8,
            SilverBotPush = 0x10,
            CratePush = 0x20
        }

        public BlockMovementState WillCollideAt(Vector2 position)
        {
            Block atPosition = grid.GetElement(position);
            BlockMovementState state = 0;
            if (isSilverBot)
            {
                var poses = MainGame.GetSilverBotPositions(position);
                foreach (var item in poses)
                {
                    state |= grid.GetElement(item).GetMovementState();
                }
            }
            else
            {
                state |= atPosition.GetMovementState();
            }
            return state;
        }

    public bool TryStep(Direction4D moveDirection, out Vector2 moveTo)
        {
            moveTo = new Vector2();
            if (moveDirection != Direction4D.None)
            {
                facing = moveDirection;
                moveTo = moveDirection.ToVector2() + gridPos;
                if (moveTo.X == -1 || moveTo.Y == -1 || moveTo.X == grid.GetLength(0) || moveTo.Y == grid.GetLength(1) || isSilverBot ? (moveTo.X == grid.GetLength(0) - 1 || moveTo.Y == grid.GetLength(1) - 1) : false)
                    return false;
                Block moveToBlock = grid[(int)moveTo.X, (int)moveTo.Y];
                Block moveToBlock1XPlus = default(Block);
                Block moveToBlock1YPlus = default(Block);
                if (isSilverBot)
                {
                    moveToBlock1XPlus = grid[(int)moveTo.X + 1, (int)moveTo.Y];
                    moveToBlock1YPlus = grid[(int)moveTo.X, (int)moveTo.Y + 1];
                }
                if (moveToBlock == Block.Crate)
                {
                    Vector2 crateMovePlus = moveDirection.ToVector2();
                    if (IsTouchingSilverBot(crateMovePlus + moveTo))
                        return false;
                    if (!grid[(int)(moveTo.X + crateMovePlus.X), (int)(moveTo.Y + crateMovePlus.Y)].IsSolid())
                    {
                        grid[(int)(moveTo.X + crateMovePlus.X), (int)(moveTo.Y + crateMovePlus.Y)] = Block.Crate;
                        grid[(int)(moveTo.X), (int)moveTo.Y] = Block.Floor;
                    }
                    else
                        return false;
                }
                if (IsTouchingSilverBot(moveTo))
                    if (!game.silverBot.Push(moveDirection.ToVector2(), this))
                        return false;
                if (isSilverBot)
                    goto SkipForeach;
            SkipForeach:
                if ((!MainGame.IsSolid(moveToBlock) || moveToBlock == Block.Panel || moveToBlock == Block.Exit) && (isSilverBot ? !MainGame.IsSolid(moveToBlock1XPlus) && !MainGame.IsSolid(moveToBlock1YPlus) : true))
                {
                    gridPos = moveTo;
                    animTarget = gridPos * 32.0f;
                    if (game.silverBot == null)
                        game.ReloadLasers(uint.MaxValue, uint.MaxValue);
                    else
                        game.ReloadLasers((uint)game.silverBot.gridPos.X, (uint)game.silverBot.gridPos.Y);
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures, facing, animPos);
            if( explosionAnimStopwatch > 0.0f )
            {
                spriteBatch.Draw(explosionTexture, new Rectangle(
                    (int)(animPos.X + (MainGame.widthOfBlock - explosionAnimStopwatch)/ 2.0f),
                    (int)(animPos.Y + (MainGame.heightOfBlock - explosionAnimStopwatch) / 2.0f),
                    (int)explosionAnimStopwatch,
                    (int)explosionAnimStopwatch), Color.White);
            }
        }
    }
}
