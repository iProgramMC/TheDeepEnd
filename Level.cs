using Microsoft.Xna.Framework;
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
        public const float springSpecialEffectTime = 1.0f;

        // access: tiles[X, Y]
        public eTileType[,] tiles = new eTileType[MainGame.levelWidth, MainGame.levelHeight * 2];
        public Dictionary<Point, float> specialEffectTimers = new Dictionary<Point, float>();
        public readonly MainGame mainGame;

        public Level(MainGame mg)
        {
            mainGame = mg;
        }

        public void SpecialEffect(Point p, float t)
        {
            specialEffectTimers[p] = t;
        }

        public void UpdateSpecialEffects(GameTime gt)
        {
            List<Point> lp = new List<Point>();
            foreach (var kvp in specialEffectTimers)
                lp.Add(kvp.Key);

            foreach (Point p in lp)
            {
                float f = specialEffectTimers[p] - Hax.Elapsed(gt);
                if (f <= 0) {
                    specialEffectTimers.Remove(p);
                }

                specialEffectTimers[p] = f;
            }
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

        public void PlacePrefabSlice(int y, int ySliceOfPrefab, Prefab prefab, ref int spawnPointX)
        {
            spawnPointX = -1;
            for (int xo = 0; xo < levelWidth; xo++)
            {
                char chr = prefab.data[ySliceOfPrefab * levelWidth + xo];
                if (chr == 'S') spawnPointX = xo;
                SetTile(xo, y, LevelPrefabs.CharToTileType(chr));
            }
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

        public float GetSpecialEffectTimer(Point point)
        {
            if (specialEffectTimers.ContainsKey(point))
                return specialEffectTimers[point];

            return 0;
        }
    }
}
