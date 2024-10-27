using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public class BroomStick : Collectable
    {
        public BroomStick(Level level, Vector2 position) :
            base(level, new Vector2(8, 8), position)
        { }

        public override void Collect(Player player)
        {
            base.Collect(player);
            player.doubleJumpEnabled = true;
            level.mainGame.AddScore(2000);
            level.mainGame.sfx_upgrade.Play();
        }

        public override Point GetFrameInActors()
        {
            return new Point(1, 2);
        }
    }
}
