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
        BackgroundManager _backgroundManager = new BackgroundManager();
        MovableEntityManager _movableEntityManager = new MovableEntityManager();
        CollisionManager _collisionManager = new CollisionManager();
        EvolutionManager _evolutionManager = new EvolutionManager();
        QuestProgressManager _questProgressManager = new QuestProgressManager();
        SoundEffectManager _soundEffectManager = new SoundEffectManager();
        StaticScreenManager _staticScreenManager;

        // A list for sorting so we can draw correctly
        List<Entity> _allEntities = new List<Entity>();

        // The house rectangle, to keep things simple
        Rectangle _houseEnterArea = new Rectangle(-1, -1, -1, -1);
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
            SharedContext.SpriteBatch = spriteBatch;
            SharedContext.MovableEntityManager = _movableEntityManager;
            SharedContext.EvolutionManager = _evolutionManager;
            SharedContext.QuestProgressManager = _questProgressManager;
            SharedContext.SoundEffectManager = _soundEffectManager;

            _staticScreenManager = new StaticScreenManager();

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

            if (_staticScreenManager.Update(gameTime)) return;

            // Return if the player dies
            if (_movableEntityManager.Hero.Health <= 0) return;

            // TODO: Add your update logic here
            _backgroundManager.Update(gameTime);
            _movableEntityManager.Update(gameTime);

            List<ICollidable> collidables = new List<ICollidable>();
            collidables.AddRange(_movableEntityManager.Cats);
            collidables.Add(_movableEntityManager.Hero);
            collidables.AddRange(_movableEntityManager.Ammo);

            _collisionManager.NotifyCollisions(collidables);

            _movableEntityManager.RemoveRequestedRemovals();

            // Build a hit box for the old man's house
            if (_houseEnterArea.Width < 0)
                _houseEnterArea = new Rectangle((int)(_backgroundManager.House.WorldPosition.X - 100), (int)(_backgroundManager.House.WorldPosition.Y - 100), 420, 300);
            
            // Check if we are near the wise man's house
            if (new Rectangle((int)(_movableEntityManager.Hero.CollisionBox.X + _movableEntityManager.Hero.WorldPosition.X),
                (int)(_movableEntityManager.Hero.CollisionBox.Y + _movableEntityManager.Hero.WorldPosition.Y),
                _movableEntityManager.Hero.CollisionBox.Width, _movableEntityManager.Hero.CollisionBox.Height).Intersects(_houseEnterArea))
            {
                _heroWithinHouseBounds = true;

                if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest)
                    _questProgressManager.QuestCompleted = true;

                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PostGame)
                    {
                        _movableEntityManager.CurrentQuest = MovableEntityManager.QuestEnum.YouWin;
                    }
                    else
                    {

                        if (_questProgressManager.QuestCompleted)
                        {
                            _movableEntityManager.CurrentQuest++;
                            _questProgressManager.QuestCompleted = false;

                            // Auto pick the upgraded weapon
                            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
                            {
                                _movableEntityManager.Hero.HeroWeapon = Hero.HeroWeaponEnum.ShotGun;
                            }
                            else if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
                            {
                                _movableEntityManager.Hero.HeroWeapon = Hero.HeroWeaponEnum.UpgradedShotGun;
                            }
                        }
                    }

                    _staticScreenManager.CurrentScreen = StaticScreenManager.CurrentScreenEnum.Story;
                }
            }
            else
            {
                _heroWithinHouseBounds = false;
            }

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

            if (_staticScreenManager.Draw(gameTime, spriteBatch)) return;

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, _camera.GetViewMatrix(_parralaxCamSpeed));

            // Draw the background
            _backgroundManager.Draw(gameTime, spriteBatch);

            // Draw the movable entities
            _movableEntityManager.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // Draw the HUD
            spriteBatch.Begin();

            // Draw player health
            int healthIndex = 20;
            for (int i = 0; i < 5; i++)
            {
                if (i < _movableEntityManager.Hero.Health)
                    spriteBatch.Draw(Content.Load<Texture2D>("Images/healthIndicator"), new Rectangle(healthIndex, 10, 25, 25), null, Color.White,
                        0, Vector2.Zero, SpriteEffects.None, 1);
                else
                    spriteBatch.Draw(Content.Load<Texture2D>("Images/healthIndicator_Bad"), new Rectangle(healthIndex, 10, 25, 25), null,
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);

                healthIndex += 25;
            }

            // Game Over
            if (_movableEntityManager.Hero.Health <= 0)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Images/youDied"), new Rectangle((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - 350),
                    (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - 250, 500, 300), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);


                if (_movableEntityManager.Hero.Health <= 0)
                {
                    if (Keyboard.GetState().GetPressedKeys().Contains(Keys.R))
                    {
                        RestartGame();
                    }
                }
            }

            // Draw the Weapons and selection
            int weaponMargin = GraphicsDevice.Viewport.Width - 250;
            spriteBatch.Draw(Content.Load<Texture2D>("Images/sword"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (_movableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.Sword ? 1 : .3F),
                        0, Vector2.Zero, SpriteEffects.None, 1);
            weaponMargin += 75;


            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest ||
                _movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
            {
                // Draw no weapon
            }
            else
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Images/shotgun"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (_movableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.ShotGun ? 1 : .3F),
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }


            weaponMargin += 75;
            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest ||
                _movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1 ||
                _movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
            {
                // Draw no weapon
            }
            else
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Images/upgradedShotgun"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (_movableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.UpgradedShotGun ? 1 : .3F),
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }


            // Draw Quest Progress
            int questMargin = GraphicsDevice.Viewport.Width - 300;
            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest)
            {
                if (_questProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest: Enter the Old Man's House", new Vector2(questMargin - 110, 64), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest: Find the Old Man", new Vector2(questMargin, 64), Color.White);
                }
            }

            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
            {
                int percentComplete = (int)(((float)_questProgressManager.NumberOfInnocentKittensKilled / (float)_questProgressManager.NumberOfInnocentKittensToFinishQuest) * 100);
                if (percentComplete > 100)
                    percentComplete = 100;

                if (_questProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest Completed:", new Vector2(questMargin - 60, 64), Color.White);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("startScreenFont"), "Return to the Old Man", new Vector2(questMargin, 94), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest Progress:", new Vector2(questMargin - 50, 64), Color.White);
                }

                // Draw a progress bar
                spriteBatch.Draw(Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120, 70, percentComplete, 15), null, Color.Green * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);

                spriteBatch.Draw(Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120 + percentComplete, 70, 100 - percentComplete, 15), null, Color.LightGray * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }
            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
            {
                int percentComplete = (int)(((float)_questProgressManager.NumberOfMutatedKittensKilled / (float)_questProgressManager.NumberOfMutatedKittensToFinishQuest) * 100);
                if (percentComplete > 100)
                    percentComplete = 100;

                if (_questProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest Completed:", new Vector2(questMargin - 60, 64), Color.White);
                    spriteBatch.DrawString(Content.Load<SpriteFont>("startScreenFont"), "Return to the Old Man", new Vector2(questMargin, 94), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Quest Progress:", new Vector2(questMargin - 50, 64), Color.White);
                }

                // Draw a progress bar
                spriteBatch.Draw(Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120, 70, percentComplete, 15), null, Color.Green * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);

                spriteBatch.Draw(Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120 + percentComplete, 70, 100 - percentComplete, 15), null, Color.LightGray * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }
            if (_movableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
            {
                spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Kill the Kitten Lord!", new Vector2(questMargin , 64), Color.White);
            }

            // Draw the instructions
            int y = GraphicsDevice.Viewport.Height - 100;
            spriteBatch.Draw(Content.Load<Texture2D>("Images/instructions"), new Rectangle(20, y, 588, 74), null, Color.White * .8F,
                        0, Vector2.Zero, SpriteEffects.None, 1);

            spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Press Escape to Pause", new Vector2(
                120, GraphicsDevice.Viewport.Height - 25), Color.White);

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, _camera.GetViewMatrix(_parralaxCamSpeed));

            // Build a hit box for the old man's house
            if (_houseEnterArea.Width < 0)
                _houseEnterArea = new Rectangle((int)(_backgroundManager.House.WorldPosition.X - 100), (int)(_backgroundManager.House.WorldPosition.Y - 100), 420, 300);

            // Check if we are near the wise man's house
            if (_heroWithinHouseBounds)
            {
                // Let the user know they can enter
                spriteBatch.DrawString(Content.Load<SpriteFont>("statusFont"), "Press Q to enter the Wise Man's house.",
                    new Vector2(_backgroundManager.House.WorldPosition.X - 50,
                    _backgroundManager.House.WorldPosition.Y + 150), Color.White * .7F);
            }

            // Draw a debug rectangle for the house            
            //spriteBatch.Draw(Content.Load<Texture2D>("Images/censored"), _houseEnterArea, null, Color.Red * .5F,
            //        0, Vector2.Zero, SpriteEffects.None, 1);

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
