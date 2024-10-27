using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDeepEnd
{
    public class Level
    {
        public const int levelWidth = MainGame.levelWidth;
        public const int levelHeight = MainGame.levelHeight;
        public const int tileSize = MainGame.tileSize;
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

        public void PlacePrefabSlice(int y, int ySliceOfPrefab, Prefab prefab, ref int spawnPointX, bool flipHorizontally, int actualRowY)
        {
            spawnPointX = -1;
            for (int xo = 0; xo < levelWidth; xo++)
            {
                char chr = prefab.data[ySliceOfPrefab * levelWidth + xo];
                int dx = xo;
                if (flipHorizontally)
                {
                    dx = levelWidth - 1 - xo;
                    /**/ if (chr == '>') chr = '<';
                    else if (chr == '<') chr = '>';
                }

                int entY = actualRowY * tileSize;

                char chro = '.';
                switch (chr)
                {
                    case 'S':
                        spawnPointX = dx;
                        break;

                    case 'N':
                        mainGame.AddActor(new BroomStick(this, new Vector2(dx * tileSize + 4, entY + 8)));
                        break;

                    case 'P':
                        mainGame.AddActor(new Pumpkin(this, new Vector2(dx * tileSize + 4, entY + 8)));
                        break;

                    default:
                        chro = chr;
                        break;
                }

                SetTile(dx, y, LevelPrefabs.CharToTileType(chro));
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
