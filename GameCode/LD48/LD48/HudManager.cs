using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD48
{
    public class HudManager
    {

        public void Update()
        {
        }

        public void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            // Draw player health
            int healthIndex = 20;
            for (int i = 0; i < 5; i++)
            {
                if (i < SharedContext.MovableEntityManager.Hero.Health)
                    spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/healthIndicator"), new Rectangle(healthIndex, 10, 25, 25), null, Color.White,
                        0, Vector2.Zero, SpriteEffects.None, 1);
                else
                    spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/healthIndicator_Bad"), new Rectangle(healthIndex, 10, 25, 25), null,
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);

                healthIndex += 25;
            }

            // Game Over
            if (SharedContext.MovableEntityManager.Hero.Health <= 0)
            {
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/youDied"), new Rectangle((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - 350),
                    (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - 250, 500, 300), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);


                if (SharedContext.MovableEntityManager.Hero.Health <= 0)
                {
                    if (Keyboard.GetState().GetPressedKeys().Contains(Keys.R))
                    {
                        SharedContext.RestartGameTrigger = true;
                    }
                }
            }

            // Draw the Weapons and selection
            int weaponMargin = SharedContext.GraphicsDevice.Viewport.Width - 250;
            spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/sword"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (SharedContext.MovableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.Sword ? 1 : .3F),
                        0, Vector2.Zero, SpriteEffects.None, 1);
            weaponMargin += 75;


            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest ||
                SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
            {
                // Draw no weapon
            }
            else
            {
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/shotgun"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (SharedContext.MovableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.ShotGun ? 1 : .3F),
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }


            weaponMargin += 75;
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest ||
                SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1 ||
                SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
            {
                // Draw no weapon
            }
            else
            {
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/upgradedShotgun"), new Rectangle(weaponMargin, 10, 70, 50), null, Color.White * (SharedContext.MovableEntityManager.Hero.HeroWeapon == Hero.HeroWeaponEnum.UpgradedShotGun ? 1 : .3F),
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }


            // Draw Quest Progress
            int questMargin = SharedContext.GraphicsDevice.Viewport.Width - 300;
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest)
            {
                if (SharedContext.QuestProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest: Enter the Old Man's House", new Vector2(questMargin - 110, 64), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest: Find the Old Man", new Vector2(questMargin, 64), Color.White);
                }
            }

            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
            {
                int percentComplete = (int)(((float)SharedContext.QuestProgressManager.NumberOfInnocentKittensKilled / (float)SharedContext.QuestProgressManager.NumberOfInnocentKittensToFinishQuest) * 100);
                if (percentComplete > 100)
                    percentComplete = 100;

                if (SharedContext.QuestProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest Completed:", new Vector2(questMargin - 60, 64), Color.White);
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("startScreenFont"), "Return to the Old Man", new Vector2(questMargin, 94), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest Progress:", new Vector2(questMargin - 50, 64), Color.White);
                }

                // Draw a progress bar
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120, 70, percentComplete, 15), null, Color.Green * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);

                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120 + percentComplete, 70, 100 - percentComplete, 15), null, Color.LightGray * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
            {
                int percentComplete = (int)(((float)SharedContext.QuestProgressManager.NumberOfMutatedKittensKilled / (float)SharedContext.QuestProgressManager.NumberOfMutatedKittensToFinishQuest) * 100);
                if (percentComplete > 100)
                    percentComplete = 100;

                if (SharedContext.QuestProgressManager.QuestCompleted)
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest Completed:", new Vector2(questMargin - 60, 64), Color.White);
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("startScreenFont"), "Return to the Old Man", new Vector2(questMargin, 94), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Quest Progress:", new Vector2(questMargin - 50, 64), Color.White);
                }

                // Draw a progress bar
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120, 70, percentComplete, 15), null, Color.Green * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);

                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/progBar"), new Rectangle(questMargin + 120 + percentComplete, 70, 100 - percentComplete, 15), null, Color.LightGray * .8F,
                            0, Vector2.Zero, SpriteEffects.None, 1);
            }
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
            {
                spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Kill the Kitten Lord!", new Vector2(questMargin, 64), Color.White);
            }

            // Draw the instructions
            int y = SharedContext.GraphicsDevice.Viewport.Height - 100;
            spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/instructions"), new Rectangle(20, y, 588, 74), null, Color.White * .8F,
                        0, Vector2.Zero, SpriteEffects.None, 1);

            spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), "Press Escape to Pause", new Vector2(
                120, SharedContext.GraphicsDevice.Viewport.Height - 25), Color.White);

            // Draw enterable message
            foreach (string enterMessage in SharedContext.BackgroundManager.EnterableAreaMessage)
            {
                spriteBatch.DrawString(SharedContext.Content.Load<SpriteFont>("statusFont"), enterMessage, new Vector2(
                    120, 25), Color.White);
            }

        }

    }
}
