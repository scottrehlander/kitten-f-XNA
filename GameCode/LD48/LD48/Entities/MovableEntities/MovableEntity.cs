using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48
{
    public class MovableEntity : Entity, ICollidable
    {

        protected Texture2D _collisionTexture;

        protected bool _drawCollisionBoxes = false;
        protected bool _drawLineOfSiteBoxes = false;

        public virtual Rectangle CollisionBox { get; set;}

        public virtual Rectangle LineOfSite { get; set; }


        public override void LoadContent()
        {
            _collisionTexture = SharedContext.Content.Load<Texture2D>("Images/hitBoxDebug");

            base.LoadContent();                                                                                                                                                                                     
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_drawCollisionBoxes)
            {
                spriteBatch.Draw(_collisionTexture, new Rectangle((int)WorldPosition.X + CollisionBox.X, (int)WorldPosition.Y + CollisionBox.Y,
                    (int)CollisionBox.Width, (int)CollisionBox.Height), null, Color.White * 0.5F, 0, Vector2.Zero, SpriteEffects.None, 1);
            }

            if (_drawLineOfSiteBoxes)
            {
                if (LineOfSite != null)
                    spriteBatch.Draw(_collisionTexture, new Rectangle((int)WorldPosition.X + LineOfSite.X, (int)WorldPosition.Y + LineOfSite.Y,
                    (int)LineOfSite.Width, (int)LineOfSite.Height), null, Color.Red * 0.2F, 0, Vector2.Zero, SpriteEffects.None, 1);
            }

            base.Draw(gameTime, spriteBatch);
        }


        #region ICollidable

        public void CollisionDetected(ICollidable collider)
        {
            HandleCollision(collider);
        }

        public int GetUniqueEntityId()
        {
            return UniqueId;
        }

        protected virtual void HandleCollision(ICollidable collider)
        {
            // Super class should implement this
        }

        #endregion

    }
}
