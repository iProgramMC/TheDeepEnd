using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public class Level
    {
        // access: tiles[X, Y]
        public eTileType[,] tiles = new eTileType[MainGame.levelWidth, MainGame.levelHeight * 2];

        public Level()
        {
        }

        public void DrawBorder(int ix, int iy, int width, int height, eTileType tileType)
        {
            // draw a border
            for (int i = 0; i < MainGame.levelWidth; i++)
            {
                SetTile(ix + i, iy, tileType);
                SetTile(ix + i, iy + height - 1, tileType);
            }
            for (int i = 0; i < MainGame.levelHeight; i++)
            {
                SetTile(ix, iy + i, tileType);
                SetTile(ix + width - 1, iy + i, tileType);
            }
        }

        public eTileType GetTile(int x, int y)
        {
            if (x < 0 || x >= MainGame.levelWidth)
                return eTileType.Block;

            int y2 = y % (MainGame.levelHeight * 2);
            if (y2 < 0) y2 += MainGame.levelHeight * 2;
            return tiles[x, y2];
        }
        public void SetTile(int x, int y, eTileType tt)
        {
            if (x < 0 || x >= MainGame.levelWidth)
                return;

            int y2 = y % (MainGame.levelHeight * 2);
            if (y2 < 0) y2 += MainGame.levelHeight * 2;
            tiles[x, y2] = tt;
        }
    }
}
