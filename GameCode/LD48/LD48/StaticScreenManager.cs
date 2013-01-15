using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD48
{
    public class StaticScreenManager
    {

        public enum CurrentScreenEnum
        {
            None,
            Start,
            Story,
            Paused
        }
        private CurrentScreenEnum _currentScreen = CurrentScreenEnum.Start;
        public CurrentScreenEnum CurrentScreen { get { return _currentScreen; } set { _currentScreen = value; } }
        
        private SpriteFont _mainFont;


        public StaticScreenManager()
        {
        }

        public void LoadContent()
        {
            _mainFont = SharedContext.Content.Load<SpriteFont>("startScreenFont");
        }

        /// <summary>
        /// This returns true if we are handling all drawing for the moment
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public bool Update(GameTime gameTime)
        {
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.YouWin)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    SharedContext.MovableEntityManager.CurrentQuest = MovableEntityManager.QuestEnum.PostGame;
                    CurrentScreen = CurrentScreenEnum.None;
                }
            }

            if(_currentScreen == CurrentScreenEnum.None)
                return false;

            if (_currentScreen == CurrentScreenEnum.Start)
            {
                if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Space))
                {
                    _currentScreen = CurrentScreenEnum.None;
                }
            }

            if (_currentScreen == CurrentScreenEnum.Paused)
            {
                if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Space))
                {
                    _currentScreen = CurrentScreenEnum.None;
                }
            }


            if (_currentScreen == CurrentScreenEnum.Story)
            {
                SharedContext.MovableEntityManager.Hero.RestoreHealth();

                if (Keyboard.GetState().GetPressedKeys().Contains(Keys.E) || Keyboard.GetState().GetPressedKeys().Contains(Keys.Space) || Keyboard.GetState().GetPressedKeys().Contains(Keys.Escape))
                {
                    _currentScreen = CurrentScreenEnum.None;
                }
                if (Keyboard.GetState().GetPressedKeys().Contains(Keys.H))
                {
                    if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2 ||
                        SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
                    {
                        SharedContext.MovableEntityManager.Hero.RestoreHealth();
                    }
                }
            }

            return true;
        }

        public bool Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.YouWin)
            {
                spriteBatch.Begin();

                //Draw some text
                //spriteBatch.DrawString(_mainFont, "Press start to begin!", new Vector2(100, 100), Color.Green);
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("QuestText/youWin"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375, 
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);

                spriteBatch.End();

                return true;
            }

            if (_currentScreen == CurrentScreenEnum.Start)
            {
                spriteBatch.Begin();
                
                //Draw some text
                //spriteBatch.DrawString(_mainFont, "Press start to begin!", new Vector2(100, 100), Color.Green);
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/titleScreen"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375,
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);
                                
                spriteBatch.End();

                return true;
            }
            
            if (_currentScreen == CurrentScreenEnum.Paused)
            {
                spriteBatch.Begin();

                //Draw some text
                //spriteBatch.DrawString(_mainFont, "Press start to begin!", new Vector2(100, 100), Color.Green);
                spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("Images/paused"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375,
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);

                spriteBatch.End();

                return true;
            }

            if (_currentScreen == CurrentScreenEnum.Story)
            {
                spriteBatch.Begin();
                
                if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest1)
                {
                    spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("QuestText/quest1"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375,
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);
                }
                if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest2)
                {
                    spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("QuestText/quest2"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375,
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);
                }
                if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.Quest3)
                {
                    spriteBatch.Draw(SharedContext.Content.Load<Texture2D>("QuestText/quest3"), new Rectangle(SharedContext.GraphicsDevice.Viewport.Width / 2 - 375,
                    SharedContext.GraphicsDevice.Viewport.Height / 2 - 250, 750, 500), Color.White);
                }

                spriteBatch.End();

                return true;
            }

            return false;
        }

    }
}
