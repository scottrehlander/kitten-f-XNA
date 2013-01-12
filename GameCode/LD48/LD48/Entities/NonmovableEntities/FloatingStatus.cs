using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class FloatingStatus : Entity
    {

        SpriteFont _font;
        
        float _floatSpeed = -.7F;
        string _statusText;
        Color _statusTextColor;

        TimeSpan _timeToLive = TimeSpan.FromMilliseconds(600);
        TimeSpan _timeAlive = TimeSpan.Zero;

        public FloatingStatus(string statusText, Color color)
        {
            _statusText = statusText;
            _statusTextColor = color;

            LoadContent();
        }


        public override void LoadContent()
        {
            _font = SharedContext.Content.Load<SpriteFont>("statusFont");
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Float upwards
            WorldPosition = new Vector2(WorldPosition.X, WorldPosition.Y + _floatSpeed);

            _timeAlive += gameTime.ElapsedGameTime;

            if (_timeAlive > _timeToLive)
                SharedContext.MovableEntityManager.RequestedItemsToRemove.Add(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _statusText, WorldPosition, _statusTextColor, 0, 
                Vector2.Zero, 1, SpriteEffects.None, 1);
        }
    }
}
