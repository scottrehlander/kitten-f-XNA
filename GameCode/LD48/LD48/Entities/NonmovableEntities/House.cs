using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class House : Entity, IsActionable
    {

        private Rectangle _enterableArea;
        public Rectangle EnterableArea { get { return _enterableArea; } }
        public string EnterMessage { get { return "Press Z to Enter"; } }
        public Action ActionToExecute
        {
            get 
            {
                return new Action(() => {
                        if (SharedContext.MovableEntityManager.CurrentQuest == MovableEntityManager.QuestEnum.PreQuest)
                        {
                            SharedContext.MovableEntityManager.CurrentQuest = MovableEntityManager.QuestEnum.Quest1;
                        }
                        else if (SharedContext.QuestProgressManager.QuestCompleted)
                        {
                            SharedContext.MovableEntityManager.CurrentQuest++;

                            SharedContext.MovableEntityManager.Hero.SwitchToWeaponForQuest(SharedContext.MovableEntityManager.CurrentQuest);
                        }

                        SharedContext.StaticScreenManager.CurrentScreen = StaticScreenManager.CurrentScreenEnum.Story;
                    });
            }
        }
        

        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/house");

            _enterableArea = new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 300, 100);
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 360) + 99999) * 100 / 100000000;

            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 500, 400), null, Color.White, 0, 
                Vector2.Zero, SpriteEffects.None, layerDepth);
        }

    }
}
