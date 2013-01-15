using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LD48
{
    public class ZoneInstanceBackgroundManager
    {

        Texture2D _cementTile;
        public List<Entity> ImmovableEntities { get; set; }
        public Dictionary<string, Texture2D> _backgroundAssets;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            // TODO: Add your initialization logic here
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent()
        
            ImmovableEntities = new List<Entity>();

            ImmovableEntities.AddRange(ZoneInstanceUtils.LoadZone("1_1"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO: Add your drawing code here
            
            // Draw a shitload of cement tiles for now
            for (int i = -15; i < 15; i++)
            {
                for (int j = -15; j < 15; j++)
                {
                    if (i < -10 || i > 10 ||
                        j < -10 || j > 10)
                    {
                        spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/bg/bgTileRock"), new Rectangle(i * 150, j * 150, 150, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(_grassTile, new Rectangle(i * 150, j * 150, 150, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
            }

            // Draw some trees and a house
            foreach (Entity ent in ImmovableEntities)
                ent.Draw(gameTime, spriteBatch);
        }

    }
}
