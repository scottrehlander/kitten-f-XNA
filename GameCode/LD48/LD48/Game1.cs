using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using LD48.InputTypes;

namespace LD48
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera _camera;
        Vector2 _parralaxCamSpeed = new Vector2(1F);

        // Managers
        ZoneInstanceBackgroundManager _backgroundManager = new ZoneInstanceBackgroundManager();
        MovableEntityManager _movableEntityManager = new MovableEntityManager();
        CollisionManager _collisionManager = new CollisionManager();
        EvolutionManager _evolutionManager = new EvolutionManager();
        QuestProgressManager _questProgressManager = new QuestProgressManager();
        SoundEffectManager _soundEffectManager = new SoundEffectManager();
        StaticScreenManager _staticScreenManager = new StaticScreenManager();
        HudManager _hudManager = new HudManager();

        // Make the input a keyboard input for now
        InputManager _inputManager = new KeyboardInputManager();

        // A list for sorting so we can draw correctly
        List<Entity> _allEntities = new List<Entity>();

        // The house rectangle, to keep things simple
        bool _heroWithinHouseBounds = false;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 130;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 160;
            //graphics.IsFullScreen = true;

        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set the context
            SharedContext.GraphicsDevice = GraphicsDevice;
            SharedContext.Content = Content;
            SharedContext.MovableEntityManager = _movableEntityManager;
            SharedContext.EvolutionManager = _evolutionManager;
            SharedContext.QuestProgressManager = _questProgressManager;
            SharedContext.SoundEffectManager = _soundEffectManager;
            SharedContext.HudManager = _hudManager;
            SharedContext.InputManager = _inputManager;
            SharedContext.BackgroundManager = new ZoneInstanceBackgroundManager();

            _staticScreenManager.LoadContent();

            _camera = new Camera(GraphicsDevice.Viewport);

            // TODO: use this.Content to load your game content here
            _soundEffectManager.LoadContent();
            _backgroundManager.LoadContent();
            _movableEntityManager.LoadContent(_camera);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Check if the game needs to restart
            if (SharedContext.RestartGameTrigger)
            {
                SharedContext.RestartGameTrigger = false;
                RestartGame();
                return;
            }

            _inputManager.Update();

            // If the static screen manager handles the update, we don't need to run game update code
            if (_staticScreenManager.Update(gameTime)) return;

            // Return if the player dies
            if (_movableEntityManager.Hero.Health <= 0) return;

            // Update the background
            _backgroundManager.Update(gameTime);
            
            // Update the movable entities
            _movableEntityManager.Update(gameTime);

            // Add collidable items to a single list
            List<ICollidable> collidables = new List<ICollidable>();
            collidables.AddRange(_movableEntityManager.Cats);
            collidables.Add(_movableEntityManager.Hero);
            collidables.AddRange(_movableEntityManager.Ammo);

            // Check for collisions
            _collisionManager.NotifyCollisions(collidables);
            
            // Update the HUD
            _hudManager.Update();

            // Remove items that shouldn't be on the screen anymore
            _movableEntityManager.RemoveRequestedRemovals();

            // Pause the game if the user wants to pause
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _staticScreenManager.CurrentScreen = StaticScreenManager.CurrentScreenEnum.Paused;
            }
                        
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // If the static screen manager handled draw, don't draw the rest of the crap
            if (_staticScreenManager.Draw(gameTime, spriteBatch)) return;

            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, _camera.GetViewMatrix(_parralaxCamSpeed));

            // Draw the background
            _backgroundManager.Draw(gameTime, spriteBatch);

            // Draw the movable entities
            _movableEntityManager.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // Draw the HUD but make sure it is not relative to our camera
            spriteBatch.Begin();

            _hudManager.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void RestartGame()
        {
            _movableEntityManager.Hero.RestoreHealth();
            _movableEntityManager.CurrentQuest = MovableEntityManager.QuestEnum.PreQuest;
            _staticScreenManager.CurrentScreen = StaticScreenManager.CurrentScreenEnum.Start;
            _questProgressManager.NumberOfMutatedKittensKilled = 0;
            _questProgressManager.NumberOfInnocentKittensKilled = 0;

            _movableEntityManager.Cats.Clear();            
            EndBossBadass boss = new EndBossBadass();
            boss.LoadContent();
            _movableEntityManager.Cats.Add(boss);

            _movableEntityManager.Ammo.Clear();
            _movableEntityManager.FloatingStatuses.Clear();
            _movableEntityManager.Hero.HeroWeapon = Hero.HeroWeaponEnum.Sword;

            // Move the hero in case he's in trouble
            int yOffset = 0;
            if (_movableEntityManager.Hero.WorldPosition.Y < 0)
                yOffset = 150;
            else
                yOffset = -150;

            _movableEntityManager.Hero.WorldPosition = new Vector2(_movableEntityManager.Hero.WorldPosition.X,
                _movableEntityManager.Hero.WorldPosition.Y + yOffset);
            _movableEntityManager.Hero.Camera.Position = new Vector2(_movableEntityManager.Hero.Camera.Position.X,
                _movableEntityManager.Hero.Camera.Position.Y + yOffset);
        }

   }
}
