using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class UpgradedBuckshot : Ammo
    {
        Vector2 _direction;
        int _totalAllowableDistance = 200;
        int _totalDistanceTraveled = 0;

        int _bulletSpeed = 25;

        TimeSpan _sinceLastMove = TimeSpan.Zero;
        TimeSpan _timeUntilMoveAllowed = TimeSpan.FromMilliseconds(2);


        public UpgradedBuckshot(Vector2 direction)
        {
            _direction = direction;

            LoadContent();
        }

        public override void LoadContent()
        {
            Texture = SharedContext.Content.Load<Texture2D>("Images/bullet");

            WorldPosition = new Vector2(100, 100);

            CollisionBox = new Rectangle(0, 0, 30, 50);
            
            base.LoadContent();
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_sinceLastMove > _timeUntilMoveAllowed)
            {
                WorldPosition = new Vector2(WorldPosition.X + (_bulletSpeed * _direction.X),
                    WorldPosition.Y + (_bulletSpeed * _direction.Y));

                _totalDistanceTraveled += (int)Math.Abs(_bulletSpeed * _direction.X);
                _totalDistanceTraveled += (int)Math.Abs(_bulletSpeed * _direction.Y);

                _sinceLastMove = TimeSpan.Zero;
            }

            _sinceLastMove += gameTime.ElapsedGameTime;

            if (_totalDistanceTraveled > _totalAllowableDistance)
                SharedContext.MovableEntityManager.RequestedItemsToRemove.Add(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ((WorldPosition.Y + 30) + 99999) * 100 / 100000000;

            spriteBatch.Draw(Texture, new Rectangle((int)WorldPosition.X, (int)WorldPosition.Y, 50, 50),
                null, Color.White * 0F, 0, Vector2.Zero, SpriteEffects.None, layerDepth);

            base.Draw(gameTime, spriteBatch);
        }


        protected override void HandleCollision(ICollidable collider)
        {
            if(!(collider is Hero))
                SharedContext.MovableEntityManager.RequestedItemsToRemove.Add(this);
        }
    }
}
