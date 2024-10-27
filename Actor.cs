using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
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
        public float jumpForgivenessTimer = 0f;

        public const int jumpThroughSize = 3;
        public const int tileSize = MainGame.tileSize;
        public const float jumpForgivenessTime = 0.1f;

        public Actor(Level level, Vector2 hitBoxSize, Vector2 position)
        {
            this.level = level;
            this.position = position;
            this.hitBoxSize = hitBoxSize;
        }

        public void CheckTilesInRect(ref List<Point> tileCollisions, Rectangle rect)
        {
            int x1 = (int) Math.Floor((float)rect.X / tileSize);
            int y1 = (int) Math.Floor((float)rect.Y / tileSize);
            int x2 = (int) Math.Ceiling((float)(rect.X + rect.Width - 1) / tileSize);
            int y2 = (int) Math.Ceiling((float)(rect.Y + rect.Height - 1) / tileSize);

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
        public void CheckOverlappedTiles(ref List<Point> tileCollisions, Vector2 actorPosition)
        {
            Rectangle rect = new Rectangle(
                (int) actorPosition.X,
                (int) actorPosition.Y,
                (int) hitBoxSize.X,
                (int) hitBoxSize.Y
            );
            CheckTilesInRect(ref tileCollisions, rect);
        }

        public Rectangle CollisionRect(Vector2 actorPosition)
        {
            return new Rectangle(
                (int)actorPosition.X,
                (int)actorPosition.Y,
                (int)hitBoxSize.X,
                (int)hitBoxSize.Y
            );
        }

        public bool CollideX(Point tilePosition, Vector2 actorPosition, int moveUnit)
        {
            eTileType tt = level.GetTile(tilePosition.X, tilePosition.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);

            switch (ti.collisionType)
            {
                case eCollisionType.None:
                case eCollisionType.Spring:
                case eCollisionType.JumpThrough:
                    return false;
                    
                case eCollisionType.DeadlyUp:
                    return CollisionRect(actorPosition).Intersects(QDLRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QDRRect(tilePosition));

                case eCollisionType.DeadlyDown:
                    return CollisionRect(actorPosition).Intersects(QULRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QURRect(tilePosition));

                case eCollisionType.DeadlyLeft:
                    return CollisionRect(actorPosition).Intersects(QDRRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QURRect(tilePosition));

                case eCollisionType.DeadlyRight:
                    return CollisionRect(actorPosition).Intersects(QULRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QDLRect(tilePosition));

                case eCollisionType.Solid:
                case eCollisionType.Deadly:
                    return true;

                default:
                    throw new Exception("unknown collision type !");
            }
        }

        public const int d = 3;
        public Rectangle QULRect(Point tp) => new Rectangle(tp.X * tileSize + d, tp.Y * tileSize, tileSize / 2 - d, tileSize / 2);
        public Rectangle QURRect(Point tp) => new Rectangle(tp.X * tileSize + tileSize / 2, tp.Y * tileSize, tileSize / 2 - d, tileSize / 2);
        public Rectangle QDLRect(Point tp) => new Rectangle(tp.X * tileSize + d, tp.Y * tileSize + tileSize / 2, tileSize / 2 - d, tileSize / 2);
        public Rectangle QDRRect(Point tp) => new Rectangle(tp.X * tileSize + tileSize / 2, tp.Y * tileSize + tileSize / 2, tileSize / 2 - d, tileSize / 2);

        public bool CollideY(Point tilePosition, Vector2 actorPosition, int moveUnit)
        {
            eTileType tt = level.GetTile(tilePosition.X, tilePosition.Y);
            TileInfo ti = TileManager.GetTileInfo(tt);

            switch (ti.collisionType)
            {
                case eCollisionType.None:
                case eCollisionType.Spring:
                    return false;

                case eCollisionType.JumpThrough:
                    return moveUnit > 0 && (actorPosition.Y + hitBoxSize.Y < tilePosition.Y * tileSize + jumpThroughSize);

                case eCollisionType.DeadlyUp:
                    return CollisionRect(actorPosition).Intersects(QDLRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QDRRect(tilePosition));

                case eCollisionType.DeadlyDown:
                    return CollisionRect(actorPosition).Intersects(QULRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QURRect(tilePosition));

                case eCollisionType.DeadlyLeft:
                    return CollisionRect(actorPosition).Intersects(QDRRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QURRect(tilePosition));

                case eCollisionType.DeadlyRight:
                    return CollisionRect(actorPosition).Intersects(QULRect(tilePosition)) ||
                           CollisionRect(actorPosition).Intersects(QDLRect(tilePosition));

                case eCollisionType.Solid:
                case eCollisionType.Deadly:
                    return true;

                default:
                    throw new Exception("unknown collision type !");
            }
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

                    if ((int) newPosition.Y <= level.mainGame.CameraY)
                        break;

                    CheckOverlappedTiles(ref tileCollisions, newPosition);

                    collided = false;
                    for (int i = 0; i < tileCollisions.Count; i++)
                    {
                        if (CollideY(tileCollisions[i], newPosition, moveUnit))
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
                        if (CollideX(tileCollisions[i], newPosition, moveUnit))
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

        void ProcessFloor()
        {
            if (!isGrounded)
                return;

            Vector2 newPosition = position + new Vector2(0, 1);

            List<Point> tileCollisions = new List<Point>();
            CheckOverlappedTiles(ref tileCollisions, newPosition);

            bool collided = false;
            for (int i = 0; i < tileCollisions.Count; i++)
            {
                if (CollideY(tileCollisions[i], newPosition, 1))
                {
                    collided = true;
                    break;
                }
            }

            if (!collided)
            {
                // no longer on the ground :(
                jumpForgivenessTimer = jumpForgivenessTime;
                isGrounded = false;
            }
        }

        // Updates the actor.
        public virtual void Update(MainGame mg, GameTime gameTime)
        {
            Move(velocity * Hax.Elapsed(gameTime));
            ProcessFloor();

            if (jumpForgivenessTimer > 0)
            {
                jumpForgivenessTimer -= Hax.Elapsed(gameTime);
                if (jumpForgivenessTimer < 0)
                    jumpForgivenessTimer = 0;
            }
        }

        // Gets the frame X and Y position in the actors.png sprite sheet.
        public virtual Point GetFrameInActors()
        {
            return default;
        }
    }
}
