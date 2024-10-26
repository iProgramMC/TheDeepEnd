using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeffJamGame
{
    public class Level
    {
        public const int levelWidth = MainGame.levelWidth;
        public const int levelHeight = MainGame.levelHeight;
        // access: tiles[X, Y]
        public eTileType[,] tiles = new eTileType[MainGame.levelWidth, MainGame.levelHeight * 2];

        public Level()
        {
        }

        public void PlacePrefab(int y, string prefab)
        {
            int height = prefab.Length / levelWidth;
            Debug.Assert(prefab.Length % levelWidth == 0);

            int i = 0;
            for (int yo = 0; yo < height; yo++)
            {
                for (int xo = 0; xo < levelWidth; xo++)
                    SetTile(xo, y + yo, LevelPrefabs.CharToTileType(prefab[i++]));
            }
        }

        public void PlacePrefabSlice(int y, int ySliceOfPrefab, string prefab)
        {
            for (int xo = 0; xo < levelWidth; xo++)
                SetTile(xo, y, LevelPrefabs.CharToTileType(prefab[ySliceOfPrefab * levelWidth + xo]));
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
