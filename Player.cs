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
        public const float jumpSpeed = -200.0f;
        public const float maxFall = 400.0f;
        public const float gravity = 900.0f;
        public const float halfGravityThreshold = 40.0f;
        public const float jumpHoldTime = 0.2f;

        public float jumpHoldTimer = 0f;

        public Player(Level level, Vector2 position) :
            base(level, new Vector2(8, 12), position)
        {
        }

        public override void Update(MainGame mg, GameTime gameTime)
        {
            base.Update(mg, gameTime);

            // Running and friction
            float moveX = mg.GetMoveX();
            float mult = isGrounded ? 1.0f : 0.5f;

            if (isOnSlipperySurface)
                mult *= 0.5f;

            float accel = (Math.Abs(velocity.X) > maxRun && Math.Sign(velocity.X) == Math.Sign(moveX)) ? runReduce : runAccel;
            Hax.SetFloatWithTarget(ref velocity.X, moveX * maxRun, Hax.Elapsed(gameTime) * accel * mult);

            // Jumping
            if (isGrounded && mg.JumpPressed())
            {
                mg.ConsumeJumpBuffer();
                isGrounded = false;
                velocity.Y = jumpSpeed;
                jumpHoldTimer = jumpHoldTime;
            }

            // Gravity
            if (!isGrounded)
            {
                mult = 1.0f;
                if (Math.Abs(velocity.Y) < halfGravityThreshold && mg.JumpPressed())
                    mult = 0.5f;
                Hax.SetFloatWithTarget(ref velocity.Y, maxFall, Hax.Elapsed(gameTime) * mult * gravity);
            }

            // Jump hold
            if (jumpHoldTimer > 0)
            {
                if (!mg.JumpPressed())
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

            // Facing
            if (moveX != 0)
            {
                facingLeft = moveX < 0;
            }
        }

        public override Point GetFrameInActors()
        {
            if (!isGrounded)
            {
                if (velocity.Y < 0)
                    return new Point(1, 0);
                else
                    return new Point(2, 0);
            }

            return base.GetFrameInActors();
        }
    }
}
