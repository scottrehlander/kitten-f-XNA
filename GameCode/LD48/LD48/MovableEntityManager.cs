using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LD48;

namespace LD48
{
    public class MovableEntityManager
    {

        int NUM_OF_CATS_MIN = 70;
        int NUM_OF_CATS_MAX = 90;

        public Hero Hero { get; set; }

        public List<Cat> Cats { get; set; }

        public List<Ammo> Ammo { get; set; }

        public List<FloatingStatus> FloatingStatuses { get; set; }

        public List<Entity> RequestedItemsToRemove { get; set; }

        public List<Entity> RequestedCatsToAdd { get; set; }

        public enum QuestEnum
        {
            PreQuest,
            Quest1,
            Quest2,
            Quest3,
            YouWin,
            PostGame
        }
        public QuestEnum CurrentQuest { get; set; }

        
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
        public void LoadContent(Camera camera)
        {
            // Create our hero
            Hero = new Hero(camera);
            Hero.LoadContent();

            // Create the boss
            Cats = new List<Cat>();
            EndBossBadass boss = new EndBossBadass();
            boss.LoadContent();
            Cats.Add(boss);

            Random r = new Random();
            Random r2 = new Random();
                
            // Create some roaming cats
            if (Cats.Count < NUM_OF_CATS_MIN)
            {
                InnocentCat cat = new InnocentCat()
                {
                    GestationState = Cat.GestationStateEnum.Grown
                };

                cat.GestationState = Cat.GestationStateEnum.Grown;
                cat.WorldPosition = new Vector2(r.Next(-600, 600), r2.Next(-1200, 1200));

                Cats.Add(cat);
            }

            CurrentQuest = QuestEnum.PreQuest;

            //for (int i = 0; i < 5; i++)
            //{
            //    CatEvoLevel2 cat = new CatEvoLevel2()
            //    {
            //        WorldPosition = new Vector2(r.Next(-600, 600), r2.Next(-600, 600))
            //    };
            //    Cats.Add(cat);
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    CatEvoLevel3 cat = new CatEvoLevel3()
            //    {
            //        WorldPosition = new Vector2(r.Next(-600, 600), r2.Next(-600, 600))
            //    };
            //    Cats.Add(cat);
            //}

            Ammo = new List<Ammo>();

            FloatingStatuses = new List<FloatingStatus>();

            RequestedItemsToRemove = new List<Entity>();

            RequestedCatsToAdd = new List<Entity>();
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
            // Make sure we have plenty of cats...
            // Create some roaming cats
            Random r = new Random();
            Random r2 = new Random(DateTime.Now.Millisecond);
            for (int i = Cats.Count; i < NUM_OF_CATS_MIN; i++)
            {
                InnocentCat cat = new InnocentCat()
                {
                    WorldPosition = new Vector2(r.Next(-1200, 1200), r2.Next(-1200, 1200))
                };
                Cats.Add(cat);
            }

            // Update the Hero
            Hero.Update(gameTime);

            // Update the cats
            foreach (Cat cat in Cats)
                cat.Update(gameTime);

            // Update the Ammo
            foreach (Ammo ammo in Ammo)
                ammo.Update(gameTime);
            
            foreach (FloatingStatus fs in FloatingStatuses)
                fs.Update(gameTime);

            // Check for dead cats
            List<Cat> _catsToRemove = new List<Cat>();
            foreach (Cat cat in Cats)
                if (cat.Health <= 0)
                    _catsToRemove.Add(cat);
            foreach (Cat cat in _catsToRemove)
            {
                // Update quest progress
                if (cat is InnocentCat)
                {
                    if (CurrentQuest == QuestEnum.Quest1)
                        SharedContext.QuestProgressManager.NumberOfInnocentKittensKilled++;
                }
                else if (cat is CatEvoLevel2 || cat is CatEvoLevel3)
                {
                    if (CurrentQuest == QuestEnum.Quest2)
                        SharedContext.QuestProgressManager.NumberOfMutatedKittensKilled++;
                }

                SharedContext.SoundEffectManager.PlayFart();
                Cats.Remove(cat);
            }

            foreach (Cat cat in RequestedCatsToAdd)
            {
                // Limit the number..
                ControlPopulationToAddCat();

                Cats.Add(cat);
            }
            RequestedCatsToAdd.Clear();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Hero.Draw(gameTime, spriteBatch);

            foreach (Cat cat in Cats)
                cat.Draw(gameTime, spriteBatch);

            foreach (Ammo ammo in Ammo)
                ammo.Draw(gameTime, spriteBatch);

            foreach (FloatingStatus fs in FloatingStatuses)
                fs.Draw(gameTime, spriteBatch);
        }



        #region Functional Methods

        private void ControlPopulationToAddCat()
        {
            if (Cats.Count < NUM_OF_CATS_MAX)
                return;

            Cat catToRemove = null;

            // We need to remove a cat of the lowest level,
            for (int i = 0; i < 3; i++)
            {
                // 0 removes a innocent cat, 1 removes a lvl2 cat, 2 removes a lvl3 cat
                foreach (Cat cat in Cats)
                {
                    if (i == 0)
                    {
                        if (cat is InnocentCat)
                        {
                            catToRemove = cat;

                            i = int.MaxValue - 1;
                            break;
                        }
                    }
                    if (i == 1)
                    {
                        if (cat is CatEvoLevel2)
                        {
                            catToRemove = cat;

                            i = int.MaxValue - 1;
                            break;
                        }
                    }
                    if (i == 2)
                    {
                        if (cat is CatEvoLevel3)
                        {
                            catToRemove = cat;

                            i = int.MaxValue - 1;
                            break;
                        }
                    }
                }
            }

            if (catToRemove != null)
                Cats.Remove(catToRemove);
        }

        public void RemoveRequestedRemovals()
        {
            // Remove the items that were requested for removal in the Update calls
            foreach (Entity e in RequestedItemsToRemove)
            {
                if (Ammo.Contains(e))
                    Ammo.Remove(e as Ammo);
                if (FloatingStatuses.Contains(e))
                    FloatingStatuses.Remove(e as FloatingStatus);
            }
            RequestedItemsToRemove.Clear();
        }

        #endregion

    }
}
