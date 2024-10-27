using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public class Candy : Collectable
    {
        int frame = 0;
        public Candy(Level level, Vector2 position) :
            base(level, new Vector2(8, 8), position)
        {
            frame = level.mainGame.random.Next(2);
        }

        public override void Collect(Player player)
        {
            base.Collect(player);

            level.mainGame.AddCandy(1);
            level.mainGame.sfx_candy.Play();
        }

        public override Point GetFrameInActors()
        {
            return new Point(frame + 2, 2);
        }
    }
}
