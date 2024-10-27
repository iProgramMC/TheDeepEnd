using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public enum eCollisionType
    {
        None,
        Solid,
        Deadly,
        JumpThrough,
        Spring,
    }
    public class TileInfo
    {
        public int texX, texY;
        public eCollisionType collisionType;
    }
    public enum eTileType
    {
        None,
        Block,
        Deadly_UP,
        Deadly_RIGHT,
        Deadly_LEFT,
        Deadly_DOWN,
        JumpThrough,
        Brick,
        Stone,
        WhiteBrick,
        Spring,
    }
    public static class TileManager
    {
        public static TileInfo[] tileInfo = new TileInfo[]
        {
            new TileInfo { texX = 0, texY = 0, collisionType = eCollisionType.None }, // BLANK
            new TileInfo { texX = 0, texY = 0, collisionType = eCollisionType.Solid }, // SOLID
            new TileInfo { texX = 1, texY = 0, collisionType = eCollisionType.Deadly }, // DEADLY UP
            new TileInfo { texX = 2, texY = 0, collisionType = eCollisionType.Deadly }, // DEADLY RIGHT
            new TileInfo { texX = 3, texY = 0, collisionType = eCollisionType.Deadly }, // DEADLY LEFT
            new TileInfo { texX = 4, texY = 0, collisionType = eCollisionType.Deadly }, // DEADLY DOWN
            new TileInfo { texX = 5, texY = 0, collisionType = eCollisionType.JumpThrough }, // JUMPTHROUGH
            new TileInfo { texX = 0, texY = 1, collisionType = eCollisionType.Solid }, // BRICK
            new TileInfo { texX = 1, texY = 1, collisionType = eCollisionType.Solid }, // STONE
            new TileInfo { texX = 2, texY = 1, collisionType = eCollisionType.Solid }, // WHITE BRICK
            new TileInfo { texX = 0, texY = 2, collisionType = eCollisionType.Spring }, // WHITE BRICK
        };

        public static TileInfo GetTileInfo(eTileType tt)
        {
            return tileInfo[(int) tt];
        }
    }
}
