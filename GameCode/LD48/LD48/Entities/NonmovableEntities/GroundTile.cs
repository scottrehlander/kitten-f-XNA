using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class GroundTile : Entity
    {

        public enum GroundTileType
        {
            Grass
        }
        private GroundTileType _tileType;


        public GroundTile(GroundTileType tileType)
        {
            _tileType = tileType;
        }


        public override void LoadContent()
        {
            if (_tileType == GroundTileType.Grass)
            {
                Texture = SharedContext.Content.Load<Texture2D>("Images/bg/bgTileGrass");
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 150, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

    }
}
