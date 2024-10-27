using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public class BroomStickWearOff : Actor
    {
        public BroomStickWearOff(Level level, Player player) :
            base(level, player.hitBoxSize, player.position)
        {
            velocity = player.KnockBackVelocity();
            facingLeft = player.facingLeft;
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
            return new Point(1, 2);
        }
    }
}
