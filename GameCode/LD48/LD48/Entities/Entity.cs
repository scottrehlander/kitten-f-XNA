using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace LD48
{
    public class Entity : IEquatable<Entity>
    {
        public Texture2D Texture { get; set; }
        public Vector2 WorldPosition { get; set; }

        // Set up some variables to help us randomly seed some values
        static int _autoId = 1;
        public int UniqueId { get; set; }


        public Entity()
        {
            // Set the auto id
            UniqueId = _autoId++;
        }

        
        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {            
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }


        #region IEquatible

        public bool Equals(Entity other)
        {
            if (UniqueId == other.UniqueId)
                return true;

            return false;
        }

        #endregion
    }
}
