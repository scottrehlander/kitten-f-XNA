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

        public List<Entity> ImmovableEntities { get; set; }
        public Dictionary<string, Texture2D> _backgroundAssets;

        private List<string> _enterableAreaMessages = new List<string>();
        public List<string> EnterableAreaMessage { get { return _enterableAreaMessages; } }

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
            _enterableAreaMessages.Clear();

            foreach (IsActionable actionableItem in ImmovableEntities.Where(p => p is IsActionable))
            {
                Rectangle intersectingRect = Rectangle.Intersect(new Rectangle((int)SharedContext.MovableEntityManager.Hero.WorldPosition.X, 
                   (int)SharedContext.MovableEntityManager.Hero.WorldPosition.Y, 50, 50),
                    actionableItem.EnterableArea);
                if (intersectingRect.Height > 0 || intersectingRect.Width > 0)
                {
                    _enterableAreaMessages.Add(actionableItem.EnterMessage);
                    
                    // Check input
                    if (SharedContext.InputManager.EnterActionPressed)
                    {
                        actionableItem.ActionToExecute();
                    }
                }
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw some trees and a house
            foreach (Entity ent in ImmovableEntities)
                ent.Draw(gameTime, spriteBatch);
        }

    }
}
