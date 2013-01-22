using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class ZoneInstanceUtils
    {

        public static List<Entity> LoadZone(string zoneIdentifier)
        {
            List<Entity> zone = LoadZone1_1();

            return zone;
        }

        private static List<Entity> LoadZone1_1()
        {
            List<Entity> zone = new List<Entity>();

            Random r = new Random();
            Random r2 = new Random(53);

            for (int i = 0; i < 55; i++)
            {
                Tree tree = new Tree() { WorldPosition = new Vector2(r.Next(-1200, 1200), r2.Next(-1200, 1200)) };
                tree.LoadContent();
                zone.Add(tree);
            }

            for (int i = 0; i < 4; i++)
            {
                Rock rock = new Rock() { WorldPosition = new Vector2(r.Next(-600, 600), r2.Next(-600, 600)) };
                rock.LoadContent();
                zone.Add(rock);
            }

            // Background Tiles
            for (int i = -15; i < 15; i++)
            {
                for (int j = -15; j < 15; j++)
                {
                    if (i < -10 || i > 10 ||
                        j < -10 || j > 10)
                    {
                        CementBlock block = new CementBlock() { WorldPosition = new Vector2(i * 150, j * 150) };
                        block.LoadContent();
                        zone.Add(block);
                    }
                    else
                    {
                        GroundTile tile = new GroundTile(GroundTile.GroundTileType.Grass) { WorldPosition = new Vector2(i * 150, j * 150) };
                        tile.LoadContent();
                        zone.Add(tile);
                    }
                }
            }

            House house = new House()
            {
                WorldPosition = new Vector2(0, 0)
            };
            house.LoadContent();
            zone.Add(house);

            return zone;
        }
    }
}
