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
    public class BackgroundManager
    {

        Texture2D _cementTile;
        public List<Entity> ImmovableEntities { get; set; }

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
        {
            // TODO: use this.Content to load your game content here
            _cementTile = SharedContext.Content.Load<Texture2D>("Images/bg/bgTileGrass");

            // Add some trees?
            ImmovableEntities = new List<Entity>();

            Random r = new Random();
            Random r2 = new Random(53);

            for (int i = 0; i < 55; i++)
            {
                Tree tree = new Tree() { WorldPosition = new Vector2(r.Next(-1200, 1200), r2.Next(-1200, 1200)) };
                tree.LoadContent();
                ImmovableEntities.Add(tree);
            }

            for (int i = 0; i < 4; i++)
            {
                Rock tree = new Rock() { WorldPosition = new Vector2(r.Next(-600, 600), r2.Next(-600, 600)) };
                tree.LoadContent();
                ImmovableEntities.Add(tree);
            }

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
                        spriteBatch.Draw(_cementTile, new Rectangle(i * 150, j * 150, 150, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }
            }

            // Draw some trees and a house
            foreach (Entity ent in ImmovableEntities)
                ent.Draw(gameTime, spriteBatch);
        }

    }
}
