using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class CementBlock : Entity
    {
        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/bg/bgTileRock");
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 50) + 99999) * 100 / 100000000;

            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 150, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
