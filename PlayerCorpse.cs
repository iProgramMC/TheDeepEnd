using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public class PlayerCorpse : Actor
    {
        public const float maxVelocity = -2.0f;
        public const float minXVelocity = 50.0f;

        public PlayerCorpse(Level level, Player player) :
            base(level, player.hitBoxSize, player.position)
        {
            velocity = -player.velocity * 0.5f;
            if (velocity.Y > maxVelocity)
                velocity.Y = maxVelocity;

            if (Math.Abs(velocity.X) < minXVelocity)
            {
                if (velocity.X != 0)
                    velocity.X = Math.Sign(velocity.X) * minXVelocity;
                else
                    velocity.X = (player.facingLeft ? 1 : -1) * minXVelocity;
            }

            facingLeft = !player.facingLeft;
        }

        public override void Update(MainGame mg, GameTime gameTime)
        {
            // logic copied from Actor.Move. Don't want to allow clipping
            Vector2 by = velocity * Hax.Elapsed(gameTime);
            subPixelMemory += by;
            Vector2 toMove = new Vector2(
                (int)Math.Floor(subPixelMemory.X),
                (int)Math.Floor(subPixelMemory.Y)
            );
            subPixelMemory -= toMove;
            position += toMove;

            // gravity
            Hax.SetFloatWithTarget(ref velocity.Y, Player.maxFall * 0.5f, Hax.Elapsed(gameTime) * 0.5f * Player.gravity);
        }

        public override Point GetFrameInActors()
        {
            return new Point(7, 0);
        }
    }
}
