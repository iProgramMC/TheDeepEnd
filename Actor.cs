using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public class Actor
    {
        public Vector2 position;
        public Vector2 velocity = default;
        public Vector2 subPixelMemory = default;
        public Vector2 hitBoxSize = new Vector2(8, 12);
        public bool isGrounded = false;
        public bool isOnSlipperySurface = false;
        public Level level;
        public bool facingLeft = false;
        public bool deleted = false; // marked for deletion

        public const int jumpThroughSize = 3;
        public const int tileSize = MainGame.tileSize;

        public Actor(Level level, Vector2 hitBoxSize, Vector2 position)
        {
            this.level = level;
            this.position = position;
            this.hitBoxSize = hitBoxSize;
        }

        public void CheckOverlappedTiles(ref List<Point> tileCollisions, Vector2 actorPosition)
        {
            Rectangle rect = new Rectangle(
                (int) actorPosition.X,
                (int) actorPosition.Y,
                (int) hitBoxSize.X - 1,
                (int) hitBoxSize.Y - 1
            );

            int x1 = (int) Math.Floor((float)rect.X / tileSize);
            int y1 = (int) Math.Floor((float)rect.Y / tileSize);
            int x2 = (int) Math.Ceiling((float)(rect.X + rect.Width) / tileSize);
            int y2 = (int) Math.Ceiling((float)(rect.Y + rect.Height) / tileSize);

            tileCollisions.Clear();

            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    eTileType tt = level.GetTile(x, y);
                    if (TileManager.GetTileInfo(tt).collisionType != eCollisionType.None)
                        tileCollisions.Add(new Point(x, y));
                }
            }
        }

        public bool CollideX(Point tilePosition, Vector2 actorPosition)
        {
            eTileType tt = level.GetTile(tilePosition.X, tilePosition.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);

            if (ti.collisionType == eCollisionType.None || ti.collisionType == eCollisionType.JumpThrough)
                return false;

            return true;
        }

        public bool CollideY(Point tilePosition, Vector2 actorPosition)
        {
            eTileType tt = level.GetTile(tilePosition.X, tilePosition.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);

            if (ti.collisionType == eCollisionType.None)
                return false;

            if (ti.collisionType == eCollisionType.JumpThrough)
                return actorPosition.Y + hitBoxSize.Y < tilePosition.Y * tileSize + jumpThroughSize;

            return true;
        }

        public void Move(Vector2 by)
        {
            subPixelMemory += by;
            Vector2 toMove = new Vector2(
                (int) Math.Floor(subPixelMemory.X),
                (int) Math.Floor(subPixelMemory.Y)
            );
            subPixelMemory -= toMove;

            int moveUnit, toMoveU;
            bool collided = false;
            List<Point> tileCollisions = new List<Point>();

            // Y Movement
            if (toMove.Y != 0)
            {
                moveUnit = Math.Sign(toMove.Y);
                toMoveU = (int) toMove.Y;
                while (toMoveU != 0)
                {
                    Vector2 newPosition = position;
                    newPosition.Y += moveUnit;

                    CheckOverlappedTiles(ref tileCollisions, newPosition);

                    collided = false;
                    for (int i = 0; i < tileCollisions.Count; i++)
                    {
                        if (CollideY(tileCollisions[i], newPosition))
                        {
                            collided = true;
                            break;
                        }
                    }

                    if (collided)
                    {
                        // hit a solid!
                        if (moveUnit > 0)
                        {
                            isGrounded = true;
                            velocity.Y = 0;
                        }
                        break;
                    }
                    else
                    {
                        position = newPosition;
                        toMoveU -= moveUnit;
                    }
                }
            }
            // X Movement
            if (toMove.X != 0)
            {
                moveUnit = Math.Sign(toMove.X);
                toMoveU = (int) toMove.X;
                while (toMoveU != 0)
                {
                    Vector2 newPosition = position;
                    newPosition.X += moveUnit;

                    tileCollisions.Clear();
                    CheckOverlappedTiles(ref tileCollisions, newPosition);

                    collided = false;
                    for (int i = 0; i < tileCollisions.Count; i++)
                    {
                        if (CollideX(tileCollisions[i], newPosition))
                        {
                            collided = true;
                            break;
                        }
                    }

                    if (collided)
                        break;
                    
                    position = newPosition;
                    toMoveU -= moveUnit;
                }
            }
        }

        // Updates the actor.
        public virtual void Update(MainGame mg, GameTime gameTime)
        {
            Move(velocity * Hax.Elapsed(gameTime));
        }

        // Gets the frame X and Y position in the actors.png sprite sheet.
        public virtual Point GetFrameInActors()
        {
            return default;
        }
    }
}
