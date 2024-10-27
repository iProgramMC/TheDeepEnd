using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public class Player : Actor
    {
        public const float maxRun = 150.0f;
        public const float runReduce = 500.0f;
        public const float runAccel = 1000.0f;
        public const float jumpSpeed = -140.0f;
        public const float springSpeed = -350.0f;
        public const float maxFall = 400.0f;
        public const float gravity = 900.0f;
        public const float halfGravityThreshold = 40.0f;
        public const float jumpHoldTime = 0.2f;

        public float jumpHoldTimer = 0f;
        public float walkAnimation = 0f;

        public Player(Level level, Vector2 position) :
            base(level, new Vector2(8, 12), position)
        {
        }

        void Die()
        {
            level.mainGame.StartDeathSequence();
            level.mainGame.AddActor(new PlayerCorpse(level, this));
            deleted = true;
        }

        void ProcessHorizontalInputs(MainGame mg, GameTime gameTime)
        {
            float moveX = mg.GetMoveX();
            float mult = isGrounded ? 1.0f : 0.5f;

            if (isOnSlipperySurface)
                mult *= 0.5f;

            float accel = (Math.Abs(velocity.X) > maxRun && Math.Sign(velocity.X) == Math.Sign(moveX)) ? runReduce : runAccel;
            Hax.SetFloatWithTarget(ref velocity.X, moveX * maxRun, Hax.Elapsed(gameTime) * accel * mult);
        }

        void ProcessJumpKey(MainGame mg)
        {
            if ((isGrounded || jumpForgivenessTimer > 0) && mg.JumpPressed())
            {
                mg.ConsumeJumpBuffer();
                isGrounded = false;
                velocity.Y = jumpSpeed;
                jumpHoldTimer = jumpHoldTime;
            }
        }

        void HandleGravity(MainGame mg, GameTime gameTime)
        {
            if (!isGrounded)
            {
                float mult = 1.0f;
                if (Math.Abs(velocity.Y) < halfGravityThreshold && mg.JumpHeld())
                    mult = 0.5f;
                Hax.SetFloatWithTarget(ref velocity.Y, maxFall, Hax.Elapsed(gameTime) * mult * gravity);
            }

            // Jump hold
            if (jumpHoldTimer > 0)
            {
                if (!mg.JumpHeld())
                {
                    jumpHoldTimer = 0;
                }
                else
                {
                    jumpHoldTimer -= Hax.Elapsed(gameTime);
                    velocity.Y = Math.Min(velocity.Y, jumpSpeed);
                    if (jumpHoldTimer < 0)
                        jumpHoldTimer = 0;
                }
            }
        }

        void HandleFacing(MainGame mg)
        {
            float moveX = mg.GetMoveX();

            // Facing
            if (moveX != 0)
                facingLeft = moveX < 0;
        }

        void HandleWalkAnimation(GameTime gameTime)
        {
            walkAnimation += Math.Abs(0.005f * velocity.X * Hax.Elapsed(gameTime));
            while (walkAnimation > 2f)
                walkAnimation -= 2f;
            while (walkAnimation > .2f)
                walkAnimation -= .2f;
        }
        
        bool SpringCheck(Point tilePos)
        {
            // check if the player is actually IN The spring hit box
            Rectangle rect1 = new Rectangle(tilePos.X * tileSize + 2, tilePos.Y * tileSize + tileSize - 3, tileSize - 4, 3);
            Rectangle rect2 = new Rectangle((int)position.X, (int)position.Y, (int)hitBoxSize.X, (int)hitBoxSize.Y);
            if (rect1.Intersects(rect2))
            {
                // bouncy bounce bounce!
                isGrounded = false;
                velocity.Y = springSpeed;
                jumpHoldTimer = 0.0f;
                level.SpecialEffect(tilePos, Level.springSpecialEffectTime);
                return true;
            }
            else return false;
        }

        bool OnTileTouchedHorizontally(Point tilePos, Vector2 actorPosition)
        {
            eTileType tt = level.GetTile(tilePos.X, tilePos.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);
            if (ti.collisionType == eCollisionType.None ||
                ti.collisionType == eCollisionType.JumpThrough)
                return false;

            if (Hax.IsDeadly(ti.collisionType))
            {
                if (CollideX(tilePos, actorPosition, Math.Sign(velocity.X)))
                {
                    Die();
                    return true;
                }
                else return false;
            }

            if (ti.collisionType == eCollisionType.Spring)
                return SpringCheck(tilePos);

            velocity.X = 0;
            return false;
        }

        bool OnTileTouchedVertically(Point tilePos, Vector2 actorPosition)
        {
            eTileType tt = level.GetTile(tilePos.X, tilePos.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);
            if (ti.collisionType == eCollisionType.None ||
                ti.collisionType == eCollisionType.JumpThrough)
                return false;

            if (Hax.IsDeadly(ti.collisionType))
            {
                if (CollideY(tilePos, actorPosition, Math.Sign(velocity.Y)))
                {
                    Die();
                    return true;
                }
                else return false;
            }

            if (ti.collisionType == eCollisionType.Spring)
                return SpringCheck(tilePos);

            if (velocity.Y < 0)
                velocity.Y = 0;
            return false;
        }

        void ProcessTilesWereTouching(GameTime gt)
        {
            List<Point> tileCollisions = new List<Point>();

            //Horizontal Check
            if (Math.Sign(velocity.X) != 0)
            {
                float newX = (position.X + (float) Math.Floor(subPixelMemory.X + velocity.X * Hax.Elapsed(gt)));
                CheckTilesInRect(ref tileCollisions, new Rectangle(
                    (int)newX,
                    (int)position.Y,
                    (int)hitBoxSize.X,
                    (int)hitBoxSize.Y - 1
                ));

                for (int i = 0; i < tileCollisions.Count; i++)
                {
                    if (OnTileTouchedHorizontally(tileCollisions[i], new Vector2(newX, position.Y)))
                        break;
                }
            }

            //Vertical Check
            float yToMove = (float) Math.Floor(subPixelMemory.Y + velocity.Y * Hax.Elapsed(gt));
            if (yToMove == 0f && isGrounded)
                yToMove = 1f;

            int newY = Math.Max((int)(position.Y + yToMove), level.mainGame.CameraY);

            CheckTilesInRect(ref tileCollisions, new Rectangle(
                (int) position.X,
                (int) newY,
                (int) hitBoxSize.X,
                (int) hitBoxSize.Y
            ));

            for (int i = 0; i < tileCollisions.Count; i++)
            {
                if (OnTileTouchedVertically(tileCollisions[i], new Vector2(position.X, newY)))
                    break;
            }
        }

        public override void Update(MainGame mg, GameTime gameTime)
        {
            HandleWalkAnimation(gameTime);
            ProcessHorizontalInputs(mg, gameTime);
            ProcessJumpKey(mg);
            ProcessTilesWereTouching(gameTime);
            HandleGravity(mg, gameTime);
            HandleFacing(mg);
            base.Update(mg, gameTime);
        }

        public override Point GetFrameInActors()
        {
            if (!isGrounded)
            {
                walkAnimation = 0;
                if (velocity.Y < 0)
                    return new Point(1, 0);
                else
                    return new Point(2, 0);
            }
            else if (velocity.X != 0)
            {
                int fr = (int) (walkAnimation * 20);
                if (fr == 3) fr = 1;
                return new Point (fr + 3, 0);
            }
            else
            {
                walkAnimation = 0;
            }

            return base.GetFrameInActors();
        }
    }
}
