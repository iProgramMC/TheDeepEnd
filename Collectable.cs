using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public class Collectable : Actor
    {
        float collectableTimer = 0;

        public Collectable(Level level, Vector2 hitboxSize, Vector2 position) : base(level, hitboxSize, position) { }

        public override void Update(MainGame mg, GameTime gameTime)
        {
            base.Update(mg, gameTime);

            collectableTimer += Hax.Elapsed(gameTime);
            position.Y += (float)Math.Cos(collectableTimer * Math.PI) * 0.1f;

            Rectangle rect = CollisionRect(position);

            var actors = level.mainGame.Actors;
            foreach (var actor in actors)
            {
                if (!(actor is Player)) continue;

                Player player = actor as Player;
                if (player.CollisionRect(player.position).Intersects(rect))
                    Collect(player);
            }
        }

        public virtual void Collect(Player player)
        {
            deleted = true;
        }
    }
}
